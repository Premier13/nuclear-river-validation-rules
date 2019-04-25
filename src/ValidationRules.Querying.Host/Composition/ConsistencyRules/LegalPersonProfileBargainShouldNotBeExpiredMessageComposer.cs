﻿using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.ConsistencyRules
{
    public sealed class LegalPersonProfileBargainShouldNotBeExpiredMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var legalPersonProfileReference = references.Get<EntityTypeLegalPersonProfile>();

            return new MessageComposerResult(
                orderReference,
                Resources.LegalPersonProfileBargainShouldNotBeExpired,
                legalPersonProfileReference);
        }
    }
}