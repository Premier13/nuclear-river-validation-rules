﻿using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeProject : EntityTypeBase<EntityTypeProject>
    {
        public override int Id
        {
            get { return EntityTypeIds.Project; }
        }

        public override string Description
        {
            get { return "Project"; }
        }
    }
}