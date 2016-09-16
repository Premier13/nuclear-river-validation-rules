﻿using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLegalPersonProfile : EntityTypeBase<EntityTypeLegalPersonProfile>
    {
        public override int Id { get; } = EntityTypeIds.LegalPersonProfile;
        public override string Description { get; } = nameof(EntityTypeIds.LegalPersonProfile);
    }
}