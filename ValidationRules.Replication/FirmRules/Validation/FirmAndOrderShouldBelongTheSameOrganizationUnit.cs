﻿using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, фирма которых находится не в городе назначения заказа, должна выводиться ошибка.
    /// "Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы"
    /// 
    /// Source: FirmBelongsToOrdersDestOrgUnitOrderValidationRule
    /// </summary>
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnit : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmAndOrderShouldBelongTheSameOrganizationUnit(IQuery query) : base(query, MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from fail in query.For<Order.FirmOrganiationUnitMismatch>()
                from order in query.For<Order>().Where(x => x.Id == fail.OrderId)
                from firm in query.For<Firm>().Where(x => x.Id == order.FirmId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                                       new XElement("firm",
                                                                    new XAttribute("id", firm.Id),
                                                                    new XAttribute("name", firm.Name)),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        ProjectId = order.ProjectId,

                        Result = RuleResult,
                    };


            return messages;
        }
    }
}
