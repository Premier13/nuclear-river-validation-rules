﻿using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Hosting.Common.Identities.Connections
{
    public sealed class ValidationRulesConnectionStringIdentity : IdentityBase<ValidationRulesConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 14;

        public override string Description => nameof(ValidationRulesConnectionStringIdentity);
    }
}