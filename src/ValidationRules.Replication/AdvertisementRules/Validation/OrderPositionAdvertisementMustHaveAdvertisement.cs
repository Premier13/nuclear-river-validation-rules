﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.AdvertisementRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых хотя бы одна номенклатура имеет ContentSales=ContentIsRequired, и для этой номенклатуры не указан РМ, должна выводиться ошибка:
    /// "В позиции {0} необходимо указать рекламные материалы"
    /// "В позиции {0} необходимо указать рекламные материалы для подпозиции '{1}'"
    /// </summary>
    public sealed class OrderPositionAdvertisementMustHaveAdvertisement : ValidationResultAccessorBase
    {
        public OrderPositionAdvertisementMustHaveAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from fail in query.For<Order.MissingAdvertisementReference>().Where(x => !x.AdvertisementIsOptional).Where(x => x.OrderId == order.Id)
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

                        PeriodStart = order.Start,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}