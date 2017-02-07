﻿using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class BillsPeriodShouldMatchOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BillsPeriodShouldMatchOrder;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");

            return new MessageComposerResult(orderReference, Resources.OrderCheckOrderAndBillHaveDifferentPeriods);
        }
    }
}