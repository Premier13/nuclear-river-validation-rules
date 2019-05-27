﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых не заполнено одно из одз полей, должна выводиться ошибка.
    /// "Необходимо заполнить все обязательные для заполнения поля: {0}"
    /// 
    /// Source: RequiredFieldsNotEmptyOrderValidationRule
    /// </summary>
    public sealed class OrderRequiredFieldsShouldBeSpecified : ValidationResultAccessorBase
    {
        public OrderRequiredFieldsShouldBeSpecified(IQuery query) : base(query, MessageTypeCode.OrderRequiredFieldsShouldBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from missing in query.For<Order.MissingRequiredField>().Where(x => x.OrderId == order.Id)
                where missing.Currency || missing.BranchOfficeOrganizationUnit 
                    || missing.LegalPerson || missing.LegalPersonProfile
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object>
                                        {
                                                { "currency", missing.Currency },
                                                { "branchOfficeOrganizationUnit", missing.BranchOfficeOrganizationUnit },
                                                { "legalPerson", missing.LegalPerson },
                                                { "legalPersonProfile", missing.LegalPersonProfile },
                                        },
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
