using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading;
using NuClear.ValidationRules.Import.FactConfigurations;
using NuClear.ValidationRules.Import.FactExtractors;
using NuClear.ValidationRules.Import.FactExtractors.FlowAdvModelsInfo;
using NuClear.ValidationRules.Import.FactExtractors.FlowFinancialData;
using NuClear.ValidationRules.Import.FactExtractors.FlowKaleidoscope;
using NuClear.ValidationRules.Import.FactExtractors.FlowNomenclatures;
using NuClear.ValidationRules.Import.FactExtractors.FlowPriceLists;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import
{
    public static partial class Program
    {
        private static readonly IFactExtractor[] FactExtractors =
        {
            new FactExtractor(new IXmlFactExtractor[]
            {
                new AdvModelInRubricInfoExtractor(),
                new CpcInfoExtractor(),
                new AccountExtractor(),
                new LegalEntityExtractor(),
                new LegalUnitExtractor(),
                new NomenclatureCategoryExtractor(),
                new NomenclatureElementExtractor(),
                new PriceListExtractor(),
                new CategoryExtractor(),
                new SecondRubricExtractor(),
                new RubricExtractor(),
            }),
        };

        private static readonly IEntityConfiguration[] Configurations =
        {
            new ConsumerStateConfiguration(),

            new AccountConfiguration(),
            new AccountDetailConfiguration(),
            new BranchOfficeOrganizationUnitConfiguration(),
            new BranchOfficeConfiguration(),
            new CategoryConfiguration(),
            new CategoryProjectConfiguration(),
            new CostPerClickCategoryRestrictionConfiguration(),
            new EntityNameConfiguration(),
            new LegalPersonConfiguration(),
            new LegalPersonProfileConfiguration(),
            new NomenclatureCategoryConfiguration(),
            new PositionChildConfiguration(),
            new PositionConfiguration(),
            new PriceConfiguration(),
            new PricePositionConfiguration(),
            new SalesModelCategoryRestrictionConfiguration(),
        };

        private static void Replicate(
            string database,
            string brokers,
            string[] topic,
            string[] kafka,
            bool enableRelations,
            CancellationToken token)
        {
            try
            {
                var dataConnectionFactory = new DataConnectionFactory(database, Configurations);
                var producerFactory = new ProducerFactory(dataConnectionFactory, Configurations, enableRelations);
                var partitionManager = new ProducerManager(dataConnectionFactory, producerFactory);
                var pollTimeout = TimeSpan.FromMilliseconds(100);
                var consumer = ConsumerFactory.Create(
                    brokers,
                    kafka ?? Array.Empty<string>(),
                    FactExtractors,
                    partitionManager);

                consumer.Subscribe(topic);

                while (!token.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(pollTimeout);
                    if (consumeResult != null)
                    {
                        var state = new ConsumerState
                        {
                            Topic = consumeResult.Topic,
                            Offset = consumeResult.Offset,
                            Partition = consumeResult.Partition
                        };
                        var producer = partitionManager[consumeResult.TopicPartition];
                        producer.InsertOrUpdate(consumeResult.Value.Concat(new[] {state}));
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
#if DEBUG
                throw;
#endif
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
                new Option("--enable-relations", "Skip relation events producing") {Argument = new Argument<bool>(() => false)},
            };
            command.Handler =
                CommandHandler.Create<string, string, string[], string[], bool, CancellationToken>(Replicate);

            return command;
        }
    }
}
