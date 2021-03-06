﻿using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.Hosting.Common
{
    // TODO: move to messagging\operations repos after successful testing

    // "at least once" implementation (auto-commit, but only "stored" offsets via OffsetStore)
    public sealed class KafkaMessageFlowReceiver : IKafkaMessageFlowReceiver
    {
        private readonly IConsumer<Ignore, byte[]> _consumer;
        private readonly TimeSpan _pollTimeout;
        private readonly ITracer _tracer;

        public KafkaMessageFlowReceiver(KafkaMessageFlowReceiverSettings settings, ITracer tracer)
        {
            _pollTimeout = settings.PollTimeout;
            _tracer = tracer;

            _consumer = CreateConsumer(settings, _tracer);
        }

        private static IConsumer<Ignore, byte[]> CreateConsumer(KafkaMessageFlowReceiverSettings settings, ITracer tracer)
        {
            var config = new ConsumerConfig((IDictionary<string, string>) settings.Config)
            {
                // help kafka server logs to identify node
                ClientId = Environment.MachineName,

                // manually store offsets
                // https://github.com/edenhill/librdkafka/wiki/FAQ#why-committing-each-message-is-slow
                // предполагаются что заданы дефолтные значения для параметров
                // enable.auto.commit=true
                EnableAutoOffsetStore = false,

                // не ждём, пока наберётся необходимый batchsize
                // если дошли до конца partition, то возвращаем столько сколько можем
                EnablePartitionEof = true
            };

            // включить отладку
            //config.Set("debug", "all");

            var consumer = new ConsumerBuilder<Ignore, byte[]>(config)
                .SetLogHandler((_, x) => OnLog(tracer, x))
                .SetErrorHandler((_, x) => OnError(tracer, x))
                .SetPartitionsAssignedHandler((_, x) => OnPartitionsAssigned(tracer, x, settings.TopicPartitionOffsets))
                .SetPartitionsRevokedHandler((_, x) => OnPartitionsRevoked(tracer, x))
                .SetOffsetsCommittedHandler((_, x) => OnOffsetsCommitted(tracer, x))
                .Build();

            consumer.Subscribe(settings.TopicPartitionOffsets.Select(x => x.Topic));

            tracer.Info("KafkaAudit. Topic consumer created");

            return consumer;
        }

        IReadOnlyCollection<ConsumeResult<Ignore, byte[]>> IKafkaMessageFlowReceiver.ReceiveBatch(int batchSize)
        {
            var list = new List<ConsumeResult<Ignore, byte[]>>(batchSize);
            var partitionEofCount = _consumer.Assignment.Count;

            while (true)
            {
                try
                {
                    var result = _consumer.Consume(_pollTimeout);
                    if (result != null)
                    {
                        if (result.IsPartitionEOF)
                        {
                            // PartitioEOF может прийти несколько раз, по числу активно читающих партиций
                            if (--partitionEofCount <= 0)
                            {
                                return list;
                            }
                            continue;
                        }

                        // не фильтруем по message != null
                        // есть кейсы где важно получать и heartbeat сообщения
                        list.Add(result);
                        if (list.Count == batchSize)
                        {
                            return list;
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _tracer.Warn(ex, $"KafkaAudit - error in poll loop {ex.Error}");
                }
            }
        }

        void IKafkaMessageFlowReceiver.CompleteBatch(IEnumerable<ConsumeResult<Ignore, byte[]>> batch)
        {
            var maxResults = batch
                .GroupBy(x => x.TopicPartition)
                .Select(x => x.Last());

            foreach (var maxResult in maxResults)
            {
                _consumer.StoreOffset(maxResult);
                _tracer.Info($"KafkaAudit - offset stored {maxResult.TopicPartitionOffset}");
            }
        }

        public void Dispose()
        {
            _consumer.Unsubscribe();
            _consumer.Close();

            _tracer.Info("KafkaAudit - poll loop disposed");
        }

        private static IEnumerable<TopicPartitionOffset> OnPartitionsAssigned(ITracer tracer, List<TopicPartition> partitions, IEnumerable<TopicPartitionOffset> settings) =>
            partitions.Select(partition =>
            {
                var partitionOffset = settings
                    .Where(setting => string.Equals(setting.Topic, partition.Topic, StringComparison.OrdinalIgnoreCase) &&
                                      (setting.Partition == partition.Partition || setting.Partition == Partition.Any))
                    .Select(setting => new TopicPartitionOffset(partition.Topic, partition.Partition, setting.Offset))
                    .FirstOrDefault();

                if (partitionOffset == null)
                {
                    return null;
                }

                tracer.Info($"KafkaAudit - partitions assigned: {partitionOffset}");
                return partitionOffset;
            })
            .Where(x => x != null);

        private static void OnPartitionsRevoked(ITracer tracer, List<TopicPartitionOffset> offsets)
        {
            foreach (var offset in offsets)
            {
                tracer.Info($"KafkaAudit - partitions revoked: {offset}");
            }
        }

        private static void OnOffsetsCommitted(ITracer tracer, CommittedOffsets committedOffsets)
        {
            foreach (var offset in committedOffsets.Offsets)
            {
                tracer.Info($"KafkaAudit - offset committed: {offset}");
            }
        }

        private static void OnLog(ITracer tracer, LogMessage logMessage) =>
            tracer.Info(
                $"KafkaAudit - log: Name:{logMessage.Name}, Level:{logMessage.Level}, Facility:{logMessage.Facility}, Message:{logMessage.Message}");

        private static void OnError(ITracer tracer, Error error) =>
            tracer.Warn($"KafkaAudit - error {error}");
    }
}