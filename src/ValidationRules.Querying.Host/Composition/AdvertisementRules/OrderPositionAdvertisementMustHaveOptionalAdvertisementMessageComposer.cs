﻿using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.AdvertisementRules
{
    public sealed class OrderPositionAdvertisementMustHaveOptionalAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustHaveOptionalAdvertisement;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderPosition = (OrderPositionNamedReference)references.GetMany<EntityTypeOrderPosition>().First();
            var orderPositionAdvertisement = references.GetMany<EntityTypeOrderPosition>().Last();

            if (orderPosition.Name == orderPositionAdvertisement.Name)
            {
                return new MessageComposerResult(orderPosition.Order,
                                                 Resources.OrderCheckPositionMustHaveOptionalAdvertisements,
                                                 orderPosition);
            }

            return new MessageComposerResult(orderPosition.Order,
                                             Resources.OrderPositionAdvertisementMustHaveOptionalAdvertisement,
                                             orderPosition,
                                             orderPositionAdvertisement.Name);
        }
    }
}