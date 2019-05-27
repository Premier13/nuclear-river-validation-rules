﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ProjectRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, в позиции которого выставлена ставка меньше минимально допустимой для этого города/рубрики, должна выводиться ошибка.
    /// "Для позиции {0} в рубрику {1} указан CPC меньше минимального"
    /// 
    /// Source: CostPerClickOrderValidationRule
    /// </summary>
    public sealed class OrderPositionCostPerClickMustNotBeLessMinimum : ValidationResultAccessorBase
    {
        public OrderPositionCostPerClickMustNotBeLessMinimum(IQuery query) : base(query, MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from bid in query.For<Order.CostPerClickAdvertisement>().Where(x => x.OrderId == order.Id)
                from project in query.For<Project>().Where(x => x.Id == order.ProjectId)
                from restrictionViolated in query.For<Project.CostPerClickRestriction>().Where(x => x.ProjectId == order.ProjectId && x.CategoryId == bid.CategoryId && x.Minimum > bid.Bid && x.Begin < order.End && order.Begin < x.End)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(bid.CategoryId),
                                    new Reference<EntityTypeOrderPosition>(bid.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(bid.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.Begin > restrictionViolated.Begin ? order.Begin : restrictionViolated.Begin,
                        PeriodEnd = order.End < restrictionViolated.End ? order.End : restrictionViolated.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}