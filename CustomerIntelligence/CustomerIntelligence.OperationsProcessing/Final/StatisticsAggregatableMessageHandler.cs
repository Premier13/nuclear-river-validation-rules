﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.River.Common.Metadata.Model;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class StatisticsAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly IAggregatesConstructor _aggregatesConstructor;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public StatisticsAggregatableMessageHandler(IAggregatesConstructor aggregatesConstructor, ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
            _aggregatesConstructor = aggregatesConstructor;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages.OfType<OperationAggregatableMessage<IOperation>>())
                {
                    _aggregatesConstructor.Execute(message.Operations);
                    _telemetryPublisher.Publish<StatisticsProcessedOperationCountIdentity>(message.Operations.Count);

                    _telemetryPublisher.Publish<StatisticsProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating statistics");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}