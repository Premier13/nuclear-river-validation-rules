﻿using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм, у которых недостаёт выбранных в белый список РМ, должна выводиться ошибка при массовой проверке и предупреждение при единичной.
    /// "Для фирмы {0} не указан рекламный материал в белый список"
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementForWhitelistDoesNotSpecified
    /// 
    /// * Поскольку проверок фирм нет, то сообщения выводим в заказах этой фирмы, в которых есть как минимум один РМ с возможностью выбора в белый список.
    /// * "Недостаёт" - значит, в выпуск выходит как минимум один РМ с возможностью выбора в белый список, но ни одного выбранного.
    /// </summary>
    public sealed class WhiteListAdvertisementMustPresent : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public WhiteListAdvertisementMustPresent(IQuery query) : base(query, MessageTypeCode.WhiteListAdvertisementMustPresent)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                from project in query.For<Order.LinkedProject>().Where(x => x.OrderId == order.Id)
                from uncoveredPeriod in query.For<Firm.WhiteListDistributionPeriod>()
                                                         .Where(x => x.FirmId == order.FirmId && x.Start < order.EndDistributionDatePlan && order.BeginDistributionDate < x.End)
                                                         .Where(x => x.ProvidedByOrderId == null)
                where !order.ProvideWhiteListAdvertisement
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)),
                                new XElement("firm",
                                    new XAttribute("id", order.FirmId),
                                    new XAttribute("name", query.For<Firm>().Single(x => x.Id == order.FirmId).Name)))),
                        PeriodStart = uncoveredPeriod.Start,
                        PeriodEnd = uncoveredPeriod.End,
                        ProjectId = project.ProjectId,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}