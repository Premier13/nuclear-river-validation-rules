﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для прайс-листов, в которых для позиций с контроллируемым количеством не указан минимум должна выводиться ошибка.
    /// "В позиции прайса {0} необходимо указать минимальное количество рекламы в выпуск"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/PricePositionHasNoMinAdvertisementAmount
    /// Ошибка на самом деле выводится не для позиции прайса, а для номенклатурной позиции в прайсе, но такой сущности попросту нет.
    /// </summary>
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecified : ValidationResultAccessorBase
    {
        public MinimalAdvertisementRestrictionShouldBeSpecified(IQuery query) : base(query, MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from restriction in query.For<Price.AdvertisementAmountRestriction>().Where(x => x.MissingMinimalRestriction)
                join pp in query.For<Price.PricePeriod>() on restriction.PriceId equals pp.PriceId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "name", restriction.CategoryName } },
                                    new Reference<EntityTypeProject>(pp.ProjectId))
                                .ToXDocument(),

                        PeriodStart = pp.Begin,
                        PeriodEnd = pp.End,
                        ProjectId = pp.ProjectId,
                    };

            return messages;
        }
    }
}
