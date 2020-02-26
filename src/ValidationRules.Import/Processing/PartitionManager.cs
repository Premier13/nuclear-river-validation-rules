using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Events;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Relations;
using NuClear.ValidationRules.Import.SqlStore;

namespace NuClear.ValidationRules.Import.Processing
{
    public class PartitionManager
    {
        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly IDictionary<TopicPartition, Producer> _producers;

        public PartitionManager(DataConnectionFactory dataConnectionFactory)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _producers = new Dictionary<TopicPartition, Producer>();
        }

        public Producer this[TopicPartition topicPartition]
            => _producers[topicPartition];

        public IEnumerable<TopicPartitionOffset> OnPartitionAssigned(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            using var connection = _dataConnectionFactory.Create();

            var temp = connection.CreateTable<ConsumerState>("#ConsumerState");
            temp.BulkCopy(topicPartitions.Select(x => new ConsumerState {Topic = x.Topic, Partition = x.Partition}));
            var states = connection.GetTable<ConsumerState>()
                .Join(temp, x => new {x.Topic, x.Partition}, x => new {x.Topic, x.Partition}, (storedState, _) => storedState)
                .ToDictionary(x => new TopicPartition(x.Topic, x.Partition), x => x.Offset);

            var result = new List<TopicPartitionOffset>(topicPartitions.Count);

            foreach (var topicPartition in topicPartitions)
            {
                _producers[topicPartition] =
                    Producer.Create(_dataConnectionFactory, ConfigureProducer, EventFactory);

                var tpo = states.TryGetValue(topicPartition, out var offset)
                    ? new TopicPartitionOffset(topicPartition, offset)
                    : new TopicPartitionOffset(topicPartition, Offset.Beginning);
                Log.Debug("Producer created",
                    new {tpo.Topic, Partition = (int) tpo.Partition, Offset = (int) tpo.Offset});
                result.Add(tpo);
            }

            return result;
        }

        public void OnPartitionRevoked(IReadOnlyCollection<TopicPartitionOffset> topicPartitionOffsets)
        {
            foreach (var tpo in topicPartitionOffsets)
            {
                _producers[tpo.TopicPartition].Dispose();
                Log.Debug("Producer disposed",
                    new {tpo.Topic, Partition = (int) tpo.Partition, Offset = (int) tpo.Offset});
            }
        }

        // todo: этот код надо вынести
        private static void ConfigureProducer(ProducerConfiguration config)
        {
            // todo: extract keys from schema
            //config.Add(new AccountRelationProvider(), x => x.Id);
            //config.Add(new LegalPersonRelationProvider(), x => x.Id);
            //config.Add(new LegalPersonProfileRelationProvider(), x => x.Id);

            config.Add(new DefaultRelationProvider<Account>(), x => x.Id);
            config.Add(new DefaultRelationProvider<AccountDetail>(), x => x.Id);
            config.Add(new DefaultRelationProvider<LegalPerson>(), x => x.Id);
            config.Add(new DefaultRelationProvider<LegalPersonProfile>(), x => x.Id);
            config.Add(new DefaultRelationProvider<ConsumerState>(), x => new {x.Topic, x.Partition});
        }

        // todo: и этот код надо вынести
        private static void EventFactory(DataConnection dataConnection, IReadOnlyCollection<RelationRecord> relations)
        {
            const string eventTemplate = "<event type='RelatedDataObjectOutdatedEvent'>" +
                "<type>{0}</type>" + // NuClear.ValidationRules.Storage.Model.Facts.OrderPosition
                "<relatedType>{1}</relatedType>" + // NuClear.ValidationRules.Storage.Model.Facts.Order
                "<relatedId>{2}</relatedId>" + // 1081834304155553024
                "</event>";

            var flow = Guid.Parse("9BD1C845-2574-4003-8722-8A55B1D4AE38");

            Log.Debug("Events created", new {relations.Count});

            dataConnection.BulkCopy(relations.Select(x => new EventRecord
            {
                Flow = flow,
                Content = string.Format(eventTemplate, x.Type, x.RelatedType, x.RelatedId),
            }));
        }

        public void ThrowIfBackgroundFailed()
        {
            foreach (var producer in _producers)
                producer.Value.ThrowIfBackgroundFailed();
        }
    }
}
