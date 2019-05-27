﻿using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.ConsistencyRules
{
    public sealed class OrderMustHaveActiveDealMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveDeal;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dealState = extra.ReadDealState();

            return new MessageComposerResult(
                orderReference,
                GetFormat(dealState));
        }

        private static string GetFormat(DealState dealState)
        {
            switch (dealState)
            {
                case DealState.Missing:
                    return Resources.OrderMustHaveActiveDeal_Missing;
                case DealState.Inactive:
                    return Resources.OrderMustHaveActiveDeal_Inactive;
                default:
                    throw new Exception(nameof(dealState));
            }
        }
    }
}