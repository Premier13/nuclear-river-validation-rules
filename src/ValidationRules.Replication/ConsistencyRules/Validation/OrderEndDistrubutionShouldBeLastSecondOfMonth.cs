﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых дата окончания размещения не является последей секундой месяца (с учём числа выпусков от даты начала), должна выводиться ошибка.
    /// "Указана некорректная дата окончания размещения"
    /// 
    /// Source: DistributionDatesOrderValidationRule
    /// </summary>
    public sealed class OrderEndDistrubutionShouldBeLastSecondOfMonth : ValidationResultAccessorBase
    {
        public OrderEndDistrubutionShouldBeLastSecondOfMonth(IQuery query) : base(query, MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.InvalidEndDistributionDate>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
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
