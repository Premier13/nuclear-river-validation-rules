﻿using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var categoryReference = references.Get("category");
            var dto = message.ReadOversalesMessage();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.TooManyAdvertisementForCategory, dto.Count, dto.Max),
                categoryReference);
        }
    }
}