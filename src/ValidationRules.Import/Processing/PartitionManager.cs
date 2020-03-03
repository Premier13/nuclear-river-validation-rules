using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Relations;

namespace NuClear.ValidationRules.Import.Processing
{
    public class PartitionManager
    {
        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly ProducerFactory _producerFactory;
        private readonly IDictionary<TopicPartition, Producer> _producers;

        public PartitionManager(
            DataConnectionFactory dataConnectionFactory,
            ProducerFactory producerFactory)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _producerFactory = producerFactory;
            _producers = new Dictionary<TopicPartition, Producer>();
        }

        public Producer this[TopicPartition topicPartition]
            => _producers[topicPartition];

        public IEnumerable<TopicPartitionOffset> OnPartitionAssigned(
            IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            using var connection = _dataConnectionFactory.Create();

            var temp = connection.CreateTable<ConsumerState>("#ConsumerState");
            temp.BulkCopy(topicPartitions.Select(x => new ConsumerState {Topic = x.Topic, Partition = x.Partition}));
            var states = connection.GetTable<ConsumerState>()
                .Join(temp, x => new {x.Topic, x.Partition}, x => new {x.Topic, x.Partition},
                    (storedState, _) => storedState)
                .ToDictionary(x => new TopicPartition(x.Topic, x.Partition), x => x.Offset);

            var result = new List<TopicPartitionOffset>(topicPartitions.Count);

            foreach (var topicPartition in topicPartitions)
            {
                _producers[topicPartition] = _producerFactory.Create();

                var tpo = states.TryGetValue(topicPartition, out var offset)
                    ? new TopicPartitionOffset(topicPartition, offset)
                    : new TopicPartitionOffset(topicPartition, Offset.Beginning);
                Log.Info("Producer created",
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
                Log.Info("Producer disposed",
                    new {tpo.Topic, Partition = (int) tpo.Partition, Offset = (int) tpo.Offset});
            }
        }

        public void ThrowIfBackgroundFailed()
        {
            foreach (var producer in _producers)
                producer.Value.ThrowIfBackgroundFailed();
        }
    }
}
