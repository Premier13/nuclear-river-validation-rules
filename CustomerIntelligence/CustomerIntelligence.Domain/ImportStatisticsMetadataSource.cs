﻿using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{

    public class ImportStatisticsMetadataSource : MetadataSourceBase<ImportStatisticsMetadataIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ImportStatisticsMetadataSource()
        {
            HierarchyMetadata importStatisticsMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ImportStatisticsMetadataIdentity>())
                    .Childs(ImportStatisticsMetadata<Bit::FirmCategoryStatistics, FirmStatisticsDto>
                                .Config
                                .HasSource(Specs.Map.Bit.FirmCategoryStatistics())
                                .Aggregated(Specs.Find.Bit.FirmCategoryStatistics.ByBitDto)
                                .LeadsToProjectStatisticsCalculation(),

                            ImportStatisticsMetadata<Bit::ProjectCategoryStatistics, CategoryStatisticsDto>
                                .Config
                                .HasSource(Specs.Map.Bit.ProjectCategoryStatistics())
                                .Aggregated(Specs.Find.Bit.ProjectCategoryStatistics.ByBitDto)
                                .LeadsToProjectStatisticsCalculation());

            _metadata = new Dictionary<Uri, IMetadataElement> { { importStatisticsMetadataRoot.Identity.Id, importStatisticsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
            => _metadata;
    }
}