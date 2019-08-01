﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у юрлиц которых нет ни одного профиля, должна выводиться ошибка.
    /// "У юр. лица клиента отсутствует профиль"
    /// 
    /// Source: LegalPersonProfilesOrderValidationRule
    /// </summary>
    public sealed class LegalPersonShouldHaveAtLeastOneProfile : ValidationResultAccessorBase
    {
        public LegalPersonShouldHaveAtLeastOneProfile(IQuery query) : base(query, MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.HasNoAnyLegalPersonProfile>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
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
