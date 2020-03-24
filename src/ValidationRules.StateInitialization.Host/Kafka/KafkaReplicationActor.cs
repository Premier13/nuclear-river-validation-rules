using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Confluent.Kafka;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using Microsoft.SqlServer.Management.Smo;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.StateInitialization.Core.Storage;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Hosting.Common;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    internal sealed class KafkaReplicationActor : IActor
    {
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;
        private readonly KafkaMessageFlowInfoProvider _kafkaMessageFlowInfoProvider;
        private readonly IBulkCommandFactory<ConsumeResult<Ignore, byte[]>> _commandFactory;
        private readonly ITracer _tracer;

        private readonly IAccessorTypesProvider _accessorTypesProvider = new InMemoryAccessorTypesProvider();

        public KafkaReplicationActor(
            IConnectionStringSettings connectionStringSettings,
            IKafkaMessageFlowReceiverFactory kafkaMessageFlowReceiverFactory,
            KafkaMessageFlowInfoProvider kafkaMessageFlowInfoProvider,
            IBulkCommandFactory<ConsumeResult<Ignore, byte[]>> commandFactory,
            ITracer tracer)
        {
            _connectionStringSettings = connectionStringSettings;
            _receiverFactory = kafkaMessageFlowReceiverFactory;
            _kafkaMessageFlowInfoProvider = kafkaMessageFlowInfoProvider;
            _commandFactory = commandFactory;
            _tracer = tracer;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            foreach (var kafkaCommand in commands.OfType<KafkaReplicationCommand>())
            {
                var dataObjectTypes = kafkaCommand.ReplicateInBulkCommand.TypesToReplicate;

                using var targetConnection = CreateDataConnection(kafkaCommand.ReplicateInBulkCommand.TargetStorageDescriptor);
                
                LoadDataFromKafka2Db(kafkaCommand.MessageFlow,
                    dataObjectTypes,
                    targetConnection,
                    kafkaCommand.BatchSize,
                    (int)kafkaCommand.ReplicateInBulkCommand.BulkCopyTimeout.TotalSeconds);

                if (!kafkaCommand.ReplicateInBulkCommand.DbManagementMode.HasFlag(DbManagementMode.UpdateTableStatistics))
                {
                    continue;
                }

                IReadOnlyCollection<ICommand> updateStatisticsCommands =
                    dataObjectTypes.Select(t => kafkaCommand.ReplicateInBulkCommand.TargetStorageDescriptor.MappingSchema.GetTableName(t))
                        .Select(table => new UpdateTableStatisticsActor.UpdateTableStatisticsCommand(table,
                            StatisticsTarget.All,
                            StatisticsScanType.FullScan))
                        .ToList();
                var updateStatisticsActor = new UpdateTableStatisticsActor((SqlConnection)targetConnection.Connection);
                updateStatisticsActor.ExecuteCommands(updateStatisticsCommands);
            }

            return Array.Empty<IEvent>();
        }

        private void LoadDataFromKafka2Db(IMessageFlow messageFlow,
                                          IReadOnlyCollection<Type> dataObjectTypes,
                                          DataConnection dataConnection,
                                          int batchSize,
                                          int bulkReplaceCommandTimeoutSec)
        {
            var actors = CreateActors(dataObjectTypes,
                                      dataConnection,
                                      new BulkCopyOptions
                                      {
                                          BulkCopyTimeout = bulkReplaceCommandTimeoutSec
                                      });

            var initialStats = _kafkaMessageFlowInfoProvider.GetFlowStats(messageFlow)
                .ToDictionary(x => x.TopicPartition, x => new MessageFlowStats (x.TopicPartition, x.End, Offset.Unset));

            using var receiver = _receiverFactory.Create(messageFlow);

            while(true)
            {
                var batch = receiver.ReceiveBatch(batchSize);
                
                var bulkCommands = _commandFactory.CreateCommands(batch);
                if (bulkCommands.Count > 0)
                {
                    using var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = TimeSpan.Zero });
                    foreach (var actor in actors)
                    {
                        actor.ExecuteCommands(bulkCommands);
                    }
                    scope.Complete();
                }

                receiver.CompleteBatch(batch);

                var batchStats = batch
                    .GroupBy(x => x.TopicPartition)
                    .ToDictionary(x => x.Key, x => x.Max(y => y.Offset.Value));
                foreach (var batchStat in batchStats)
                {
                    if (!initialStats.TryGetValue(batchStat.Key, out var initialStat))
                    {
                        throw new KeyNotFoundException(batchStat.Key.ToString());
                    }

                    var currentStat = new MessageFlowStats(batchStat.Key, initialStat.End, batchStat.Value + 1);
                    _tracer.Info($"Topic {currentStat.TopicPartition}, End: {currentStat.End}, Offset: {currentStat.Offset}, Lag: {currentStat.Lag}");
                    
                    initialStats[batchStat.Key] = currentStat;
                }

                var complete = initialStats.Values.All(x => x.Lag <= 0); 
                if (complete)
                {
                    _tracer.Info($"Kafka state init for flow {messageFlow.Description} complete");
                    break;
                }
            }
        }

        private IReadOnlyCollection<IActor> CreateActors(IReadOnlyCollection<Type> dataObjectTypes,
                                                         DataConnection dataConnection,
                                                         BulkCopyOptions bulkCopyOptions)
        {
            var actors = new List<IActor>();

            foreach (var dataObjectType in dataObjectTypes)
            {
                var accessorTypes = _accessorTypesProvider.GetAccessorsFor(dataObjectType);
                foreach (var accessorType in accessorTypes)
                {
                    var accessor = Activator.CreateInstance(accessorType, (IQuery)null);
                    var actorType = typeof(BulkInsertInMemoryDataObjectsActor<>).MakeGenericType(dataObjectType);
                    var actor = (IActor)Activator.CreateInstance(actorType, accessor, dataConnection, bulkCopyOptions);

                    actors.Add(actor);
                }
            }

            return actors;
        }

        private DataConnection CreateDataConnection(StorageDescriptor storageDescriptor)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(storageDescriptor.ConnectionStringIdentity);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptor.MappingSchema);
            connection.CommandTimeout = (int)storageDescriptor.CommandTimeout.TotalMilliseconds;
            return connection;
        }
    }
}
