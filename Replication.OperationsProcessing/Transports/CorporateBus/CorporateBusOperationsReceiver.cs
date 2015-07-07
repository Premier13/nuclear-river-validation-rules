﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Messaging.Transports.CorporateBus.Flows;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Transports.CorporateBus
{
    public sealed class CorporateBusOperationsReceiver : MessageReceiverBase<CorporateBusPerformedOperationsMessage, IPerformedOperationsReceiverSettings>
    {
        private readonly ICorporateBusMessageFlowReceiver _corporateBusMessageFlowReceiver;
        private readonly ITracer _tracer;

        public CorporateBusOperationsReceiver(
            MessageFlowMetadata sourceFlowMetadata,
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            ICorporateBusMessageFlowReceiverFactory corporateBusMessageFlowReceiverFactory, 
            ITracer tracer) :
            base(sourceFlowMetadata, messageReceiverSettings)
        {
            _tracer = tracer;
            var messageFlow = (ICorporateBusFlow)SourceFlowMetadata.MessageFlow;
            _corporateBusMessageFlowReceiver = corporateBusMessageFlowReceiverFactory.Create(messageFlow);
        }

        protected override IReadOnlyList<CorporateBusPerformedOperationsMessage> Peek()
        {
            var batch = _corporateBusMessageFlowReceiver.ReceiveBatch(MessageReceiverSettings.BatchSize);
            var messages = batch.Select(corporateBusMessage => new CorporateBusPerformedOperationsMessage(new [] { corporateBusMessage })).ToList();
            return messages;
        }

        protected override void Complete(
            IEnumerable<CorporateBusPerformedOperationsMessage> successfullyProcessedMessages,
            IEnumerable<CorporateBusPerformedOperationsMessage> failedProcessedMessages)
        {
            if (failedProcessedMessages.Any())
            {
                _tracer.WarnFormat("CorporateBus processing stopped, some messages cannot be processed");
                return;
            }

            _corporateBusMessageFlowReceiver.CompleteBatch();
        }

        protected override void OnDispose(bool disposing)
        {
            _corporateBusMessageFlowReceiver.Dispose();
        }
    }
}