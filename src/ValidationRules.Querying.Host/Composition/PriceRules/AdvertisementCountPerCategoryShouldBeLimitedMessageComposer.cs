﻿using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.PriceRules
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var projectReference = references.Get<EntityTypeProject>();
            var categoryReference = references.Get<EntityTypeCategory>();
            var dto = extra.ReadOversalesMessage();

            return new MessageComposerResult(
                projectReference,
                Resources.AdvertisementCountPerCategoryShouldBeLimited,
                categoryReference,
                dto.Count,
                dto.Max);
        }
    }
}