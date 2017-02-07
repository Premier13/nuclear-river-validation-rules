﻿using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var project = references.Get("project");
            var pricePosition = references.Get("pricePosition");

            return new MessageComposerResult(
                project,
                Resources.PricePositionHasNoMinAdvertisementAmount,
                pricePosition);
        }
    }
}