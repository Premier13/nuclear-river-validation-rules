using System.Linq;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, размещающих рекламу в карточке другой фирмы, если для одного адреса есть продажи через разные SalesModel, должна выводиться ошибка
    /// "На адрес {0} оформлены позиции с разными моделями продаж. Конфликтующий заказ {1}"
    /// Цель: сообщить МПП, что невозможно одновременно продавать по разным SalesModel (гарантированное оказание и CPM) на один адрес
    /// </summary>
    public sealed class PartnerAdvertisementShouldNotHaveDifferentSalesModel : ValidationResultAccessorBase
    {
        public PartnerAdvertisementShouldNotHaveDifferentSalesModel(IQuery query) : base(query, MessageTypeCode.PartnerAdvertisementShouldNotHaveDifferentSalesModel)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from order in query.For<Order>()
                from fa in query.For<Order.PartnerPosition>().Where(x => x.OrderId == order.Id)
                select new
                {
                    fa.DestinationFirmAddressId,
                    fa.OrderPositionId,
                    fa.SalesModel,
                    OrderId = order.Id,
                    order.Scope,
                    order.Start,
                    order.End
                };
            
            var messages =
                from sale in sales 
                from conflict in sales
                    // размещаются на один и тот же адрес
                    .Where(x => x.DestinationFirmAddressId == sale.DestinationFirmAddressId)
                    // это не одна и та же позиция (фильтр сам-на-себя)
                    .Where(x => x.OrderPositionId != sale.OrderPositionId) 
                    // соответствующие заказы пересекаются по датам и "видят" друг друга
                    .Where(x => sale.Start < x.End && x.Start < sale.End && Scope.CanSee(sale.Scope, x.Scope))
                    // разные SalesModel
                    .Where(x => x.SalesModel != sale.SalesModel)
                select new Version.ValidationResult
                {
                    MessageParams =
                        new MessageParams(
                                new Reference<EntityTypeOrder>(sale.OrderId),
                                new Reference<EntityTypeOrder>(conflict.OrderId),
                                new Reference<EntityTypeFirmAddress>(sale.DestinationFirmAddressId))
                            .ToXDocument(),

                    // сужаем период ошибки до конфликтного
                    PeriodStart = sale.Start < conflict.Start ? conflict.Start : sale.Start,
                    PeriodEnd = sale.End < conflict.End ? sale.End : conflict.End,
                    OrderId = sale.OrderId,
                };

            return messages;
        }
    }
}