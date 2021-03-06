﻿using System;
using System.Collections.Generic;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.Erm;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ams;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Fiji;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.InfoRussia;
using NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Ruleset;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class FlowMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                .Primary(
                    MessageFlowMetadata.Config.For<KafkaFactsFlow>()
                        .Receiver<BatchingKafkaReceiverTelemetryDecorator>()
                        .To.Primary().Flow<AmsFactsFlow>().Connect()
                        .To.Primary().Flow<RulesetFactsFlow>().Connect()
                        .To.Primary().Flow<InfoRussiaFactsFlow>().Connect()
                        .To.Primary().Flow<FijiFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<AmsFactsFlow>()
                        .Accumulator<AmsFactsFlowAccumulator>()
                        .To.Primary().Flow<AmsFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<RulesetFactsFlow>()
                        .Accumulator<RulesetFactsFlowAccumulator>()
                        .To.Primary().Flow<RulesetFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<InfoRussiaFactsFlow>()
                        .Accumulator<InfoRussiaFactsFlowAccumulator>()
                        .To.Primary().Flow<InfoRussiaFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<FijiFactsFlow>()
                        .Accumulator<FijiFactsFlowAccumulator>()
                        .To.Primary().Flow<FijiFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<ErmFactsFlow>()
                        .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator<ErmFactsFlowTelemetryPublisher>>()
                        .Accumulator<ErmFactsFlowAccumulator>()
                        .Handler<ErmFactsFlowHandler>()
                        .To.Primary().Flow<ErmFactsFlow>().Connect(),

                    MessageFlowMetadata.Config.For<AggregatesFlow.AggregatesFlow>()
                        .Receiver<SqlEventReceiver<AggregatesFlow.AggregatesFlow>>()
                        .Accumulator<AggregatesFlowAccumulator>()
                        .Handler<AggregatesFlowHandler>()
                        .To.Primary().Flow<AggregatesFlow.AggregatesFlow>().Connect(),

                    MessageFlowMetadata.Config.For<MessagesFlow.MessagesFlow>()
                        .Receiver<SqlEventReceiver<MessagesFlow.MessagesFlow>>()
                        .Accumulator<MessagesFlowAccumulator>()
                        .Handler<MessagesFlowHandler>()
                        .To.Primary().Flow<MessagesFlow.MessagesFlow>().Connect()
                );

        public FlowMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> {{MetadataRoot.Identity.Id, MetadataRoot}};
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
