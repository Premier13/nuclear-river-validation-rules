using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Kafka
{
    public sealed class Consumer
    {
        public static IConsumer<Ignore, object> Create(
            string brokers,
            IEnumerable<string> kafka,
            XmlSerializer serializer,
            PartitionManager partitionManager)
        {
            var config = new ConsumerConfig(
                kafka.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]))
            {
                BootstrapServers = brokers,
                EnableAutoCommit = false,
            };

            return new ConsumerBuilder<Ignore, object>(config)
                .SetKeyDeserializer(Deserializers.Ignore)
                .SetValueDeserializer(new KafkaXmlDeserializer(serializer))
                .SetErrorHandler(ErrorHandler)
                .SetLogHandler(LogHandler)
                .SetPartitionsAssignedHandler((consumer, partitions) => partitionManager.OnPartitionAssigned(partitions))
                .SetPartitionsRevokedHandler((consumer, partitions) => partitionManager.OnPartitionRevoked(partitions))
                .Build();
        }

        private static void LogHandler(IConsumer<Ignore, object> consumer, LogMessage message)
            => Log.Info("Kafka message", message);

        private static void ErrorHandler(IConsumer<Ignore, object> consumer, Error error)
            => Log.Error("Kafka error", error);

        private sealed class KafkaXmlDeserializer : IDeserializer<object>
        {
            private readonly XmlSerializer _serializer;

            public KafkaXmlDeserializer(XmlSerializer serializer)
            {
                _serializer = serializer;
            }

            public object Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull)
                    return null;

                using var stream = new MemoryStream(data.ToArray());
                using var reader = XmlReader.Create(stream);
                return _serializer.CanDeserialize(reader)
                    ? _serializer.Deserialize(reader)
                    : new object();
            }
        }
    }
}
