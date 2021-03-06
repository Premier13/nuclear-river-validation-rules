﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ProjectRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    // Для заказов, с продажами в рубрики для которых не указана стоимость клика, должна выводиться ошибка
    // "Для позиции {0} в рубрику {1} отсутствует CPC"
    public sealed class OrderPositionCostPerClickMustBeSpecified : ValidationResultAccessorBase
    {
        public OrderPositionCostPerClickMustBeSpecified(IQuery query) : base(query, MessageTypeCode.OrderPositionCostPerClickMustBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from adv in query.For<Order.CategoryAdvertisement>().Where(x => x.SalesModel == Order.CategoryAdvertisement.CostPerClickSalesModel).Where(x => x.OrderId == order.Id)
                where !query.For<Order.CostPerClickAdvertisement>().Any(x => x.OrderPositionId == adv.OrderPositionId && x.CategoryId == adv.CategoryId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(adv.CategoryId),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(adv.OrderPositionId),
                                        new Reference<EntityTypePosition>(adv.PositionId)),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Start,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}