﻿using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Project : IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}