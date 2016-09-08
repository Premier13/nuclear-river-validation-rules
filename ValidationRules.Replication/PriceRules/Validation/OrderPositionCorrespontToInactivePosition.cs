using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// ��� �������, ������� ������� ��������� �� �� �������������� �������������� �������, ������ ���������� ������.
    /// "������� {0} ������������� ������� ������� ����� �����. ���������� ������� �������� ������� �� �������� ������������ �����-�����"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionCorrespontToInactivePosition
    /// todo: ������ �� ���� ������ �������� - ������� �������/scope �� ����� ������ ��� ���� ��������
    /// </summary>
    public sealed class OrderPositionCorrespontToInactivePosition : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionCorrespontToInactivePosition(IQuery query) : base(query, MessageTypeCode.OrderPositionCorrespontToInactivePosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // �������� ��������� ������������ ������ ������� �������
            var orderFirstPeriods = from orderPeriod1 in query.For<OrderPeriod>()
                                    from orderPeriod2 in query.For<OrderPeriod>().Where(x => orderPeriod1.OrderId == x.OrderId && orderPeriod1.Start > x.Start).DefaultIfEmpty()
                                    where orderPeriod2 == null
                                    select orderPeriod1;

            var orderFirstPeriodDtos = from orderFirstPeriod in orderFirstPeriods
                                  join order in query.For<Order>() on orderFirstPeriod.OrderId equals order.Id
                                  join period in query.For<Period>()
                                  on new { orderFirstPeriod.OrganizationUnitId, orderFirstPeriod.Start } equals new { period.OrganizationUnitId, period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,
                                      OrderNumber = order.Number,

                                      period.ProjectId,
                                      period.Start,
                                      period.End,
                                  };

            var pricePositionIsNotActiveErrors =
                from orderFirstPeriodDto in orderFirstPeriodDtos
                join orderPricePosition in query.For<OrderPricePosition>() on orderFirstPeriodDto.OrderId equals orderPricePosition.OrderId
                where !orderPricePosition.IsActive
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                                                                   new XElement("order",
                                                                                new XAttribute("id", orderFirstPeriodDto.OrderId),
                                                                                new XAttribute("number", orderFirstPeriodDto.OrderNumber)),
                                                                   new XElement("orderPosition",
                                                                                new XAttribute("id", orderPricePosition.OrderPositionId),
                                                                                new XAttribute("name", orderPricePosition.OrderPositionName)))),

                        PeriodStart = orderFirstPeriodDto.Start,
                        PeriodEnd = orderFirstPeriodDto.End,
                        ProjectId = orderFirstPeriodDto.ProjectId,

                        Result = RuleResult,
                    };

            return pricePositionIsNotActiveErrors;
        }
    }
}