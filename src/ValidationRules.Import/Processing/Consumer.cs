using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Extractors;

namespace NuClear.ValidationRules.Import.Processing
{
    public sealed class Consumer
    {
        public static IConsumer<Ignore, IEnumerable<object>> Create(
            string brokers,
            IEnumerable<string> kafka,
            IReadOnlyCollection<IFactExtractor> extractors,
            PartitionManager partitionManager)
        {
            var config = new ConsumerConfig(
                kafka.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]))
            {
                BootstrapServers = brokers,
            };

            return new ConsumerBuilder<Ignore, IEnumerable<object>>(config)
                .SetKeyDeserializer(Deserializers.Ignore)
                .SetValueDeserializer(new KafkaXmlDeserializer(extractors))
                .SetErrorHandler(ErrorHandler)
                .SetLogHandler(LogHandler)
                .SetPartitionsAssignedHandler(
                    (consumer, partitions) => partitionManager.OnPartitionAssigned(partitions))
                .SetPartitionsRevokedHandler((consumer, partitions) => partitionManager.OnPartitionRevoked(partitions))
                .Build();
        }

        private static void LogHandler(IConsumer<Ignore, IEnumerable<object>> consumer, LogMessage message)
            => Log.Info("Kafka message", message);

        private static void ErrorHandler(IConsumer<Ignore, IEnumerable<object>> consumer, Error error)
            => Log.Error("Kafka error", error);

        private sealed class KafkaXmlDeserializer : IDeserializer<IEnumerable<object>>
        {
            private readonly IReadOnlyCollection<IFactExtractor> _extractors;

            public KafkaXmlDeserializer(IReadOnlyCollection<IFactExtractor> extractors)
            {
                _extractors = extractors;
            }

            public IEnumerable<object> Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull)
                    return null;

                using var stream = new MemoryStream(data.ToArray());
                return _extractors.Aggregate(Enumerable.Empty<object>(),
                    (objects, extractor) => { stream.Position = 0; return objects.Concat(extractor.Extract(stream)); });
            }
        }
    }
}
