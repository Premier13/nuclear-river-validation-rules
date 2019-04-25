﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых хотя бы одна номенклатура имеет ContentSales=ContentIsNotRequired, и для этой номенклатуры не указан РМ, должна выводиться информационная ошибка:
    /// "В позиции {0} необходимо указать рекламные материалы"
    /// "В позиции {0} необходимо указать рекламные материалы для подпозиции '{1}'"
    /// </summary>
    public sealed class OrderPositionAdvertisementMustHaveOptionalAdvertisement : ValidationResultAccessorBase
    {
        public OrderPositionAdvertisementMustHaveOptionalAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustHaveOptionalAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from fail in query.For<Order.MissingAdvertisementReference>().Where(x => x.AdvertisementIsOptional).Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                              new Reference<EntityTypeOrderPosition>(fail.OrderPositionId,
                                                                                     new Reference<EntityTypeOrder>(order.Id),
                                                                                     new Reference<EntityTypePosition>(fail.CompositePositionId)),
                                              new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                                                                                  new Reference<EntityTypeOrderPosition>(fail.OrderPositionId),
                                                                                                  new Reference<EntityTypePosition>(fail.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}