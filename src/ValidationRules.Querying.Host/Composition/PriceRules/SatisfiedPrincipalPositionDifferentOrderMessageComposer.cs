﻿using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.PriceRules
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var principal = (OrderPositionNamedReference)references[0];
            var dependent = (OrderPositionNamedReference)references[1];

            return new MessageComposerResult(
                principal.Order,
                Resources.FirmAssociatedPositionShouldNotStayAlone,
                principal.PositionPrefix,
                principal,
                dependent.PositionPrefix,
                dependent,
                dependent.Order); // todo: почему не используется последний параметр?
        }
    }
}