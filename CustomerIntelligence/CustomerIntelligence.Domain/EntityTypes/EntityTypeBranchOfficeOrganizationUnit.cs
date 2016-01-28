﻿using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.BranchOfficeOrganizationUnit; }
        }

        public override string Description
        {
            get { return "BranchOfficeOrganizationUnit"; }
        }
    }
}