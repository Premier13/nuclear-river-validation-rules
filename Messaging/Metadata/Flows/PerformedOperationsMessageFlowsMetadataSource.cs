﻿using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;

namespace NuClear.AdvancedSearch.Messaging.Metadata.Flows
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            HierarchyMetadata performedOperationsMetadataRoot =
               PerformedOperations.Flows
                                  .Primary(MessageFlowMetadata.Config.For<PrimaryReplicate2CustomerIntelligenceFlow>()
                                                .Strategy<ReplicateToCustomerIntelligencePerformedOperationsPrimaryProcessingStrategy>()
                                                .Handler<ReplicateToCustomerIntelligenceMessageAggregatedProcessingResultHandler>()
                                                .To.Primary().Flow<PrimaryReplicate2CustomerIntelligenceFlow>().Connect);

            _metadata = new Dictionary<Uri, IMetadataElement> { { performedOperationsMetadataRoot.Identity.Id, performedOperationsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}
