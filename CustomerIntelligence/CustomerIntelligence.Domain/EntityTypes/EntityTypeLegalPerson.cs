﻿using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeLegalPerson : EntityTypeBase<EntityTypeLegalPerson>
    {
        public override int Id
        {
            get { return EntityTypeIds.LegalPerson; }
        }

        public override string Description
        {
            get { return "LegalPerson"; }
        }
    }
}