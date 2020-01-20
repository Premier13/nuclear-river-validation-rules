﻿using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.FirmRules
{
    public sealed class FirmAddressMustBeLocatedOnTheMapMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAddressMustBeLocatedOnTheMap;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var order = references.Get<EntityTypeOrder>();
            var orderPosition = references.Get<EntityTypeOrderPosition>();
            var firmAddress = references.Get<EntityTypeFirmAddress>();

            return new MessageComposerResult(
                order,
                Resources.FirmAddressMustBeLocatedOnTheMap,
                orderPosition,
                firmAddress);
        }
    }
}