﻿using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.FirmRules
{
    public sealed class PartnerAdvertisementMustNotCauseProblemsToTheAdvertiserMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.PartnerAdvertisementMustNotCauseProblemsToTheAdvertiser;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReferences = references.GetMany<EntityTypeOrder>().ToArray();
            var firmReference = references.Get<EntityTypeFirm>();
            var addressReference = references.Get<EntityTypeFirmAddress>();

            return new MessageComposerResult(
                orderReferences[0],
                Resources.PartnerAdvertisementCouldNotCauseProblemsToTheAdvertiser,
                addressReference,
                firmReference,
                orderReferences[1]);
        }
    }
}