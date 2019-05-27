﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у профиля (любого из) юрлица клиента которого указана дата доверенности меньшая чем дата подписания заказа, должно выводиться информационное сообщение.
    /// "У юр. лица клиента, в профиле {0} указана доверенность с датой окончания действия раньше даты подписания заказа"
    /// 
    /// Source: WarrantyEndDateOrderValidationRule
    /// </summary>
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpired : ValidationResultAccessorBase
    {
        public LegalPersonProfileWarrantyShouldNotBeExpired(IQuery query) : base(query, MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from expired in query.For<Order.LegalPersonProfileWarrantyExpired>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeLegalPersonProfile>(expired.LegalPersonProfileId),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
