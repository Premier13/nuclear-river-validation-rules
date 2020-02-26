using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Xml.Serialization;
using NuClear.ValidationRules.Import.Kafka;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.SqlStore;

using Account = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.Account.Account;
using LegalEntity =  NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalEntity.LegalEntity;
using LegalUnit = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalUnit.LegalUnit;
using CpcInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.CpcInfo.CpcInfo;
using AdvModelInRubricInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.AdvModelInRubricInfo.AdvModelInRubricInfo;

namespace NuClear.ValidationRules.Import
{
    public static partial class Program
    {
        private static void Replicate(string database, string brokers, string[] topic, string[] kafka,
            CancellationToken token)
        {
            try
            {
                var dataConnectionFactory = new DataConnectionFactory(database, Schema.Common);
                var partitionManager = new PartitionManager(dataConnectionFactory);
                var pollTimeout = TimeSpan.FromMilliseconds(100);
                var serializer = new XmlSerializer(typeof(Account),
                    new[] {typeof(LegalEntity), typeof(LegalUnit), typeof(CpcInfo), typeof(AdvModelInRubricInfo)});
                var consumer = Consumer.Create(
                    brokers,
                    kafka ?? Array.Empty<string>(),
                    serializer,
                    partitionManager);

                consumer.Subscribe(topic);

                while (!token.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(pollTimeout);
                    if (consumeResult != null)
                    {
                        var enumerable = consumeResult.Transform();
                        var producer = partitionManager[consumeResult.TopicPartition];
                        producer.InsertOrUpdate(enumerable);
                    }

                    partitionManager.ThrowIfBackgroundFailed();
                }

                Log.Info("Shutting down", new { });
                consumer.Unsubscribe();
                consumer.Close();
                consumer.Dispose();
                Log.Info("Replication completed", new { });
            }
            catch (Exception e)
            {
                Log.Error("Fatal error", e);
                throw;
            }
        }

        private static Command CreateReplicateCommand()
        {
            var command = new Command("replicate", "Run replication")
            {
                new Option("--database", "Database connection string") {Argument = new Argument<string>()},
                new Option("--brokers", "Kafka brokers (eg 'foo:9092,bar:6062'") {Argument = new Argument<string>()},
                new Option("--topic", "Topic to subscribe") {Argument = new Argument<string[]>()},
                new Option("--kafka", "Set librdkafka configuration property") {Argument = new Argument<string[]>()},
            };
            command.Handler =
                CommandHandler.Create<string, string, string[], string[], CancellationToken>(Replicate);

            return command;
        }
    }
}
