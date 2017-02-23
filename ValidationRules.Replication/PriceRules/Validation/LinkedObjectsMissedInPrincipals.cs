﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказа, в котором есть сопутствующая позиция и есть основная, но не удовлетворено условие ObjectBindingType.Match должна выводиться ошибка.
    /// "{0} содержит объекты привязки, отсутствующие в основных позициях"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/LinkedObjectsMissedInPrincipals
    /// 
    /// Q1: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///     Продана X в рубрику адреса (A, B). Продана Y в рубрику B. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q2: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///     Продана X в рубрику адреса (A, B). Продана Y к адресу A. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q3: Позиция Z - сопутствующая для X (без учёта), для Y (совпадение).
    ///     Проданы Z, Y (с другим объектом привязки), X. Должна ли появиться ошибка?
    /// A: Сейчас ERM выдаёт ошибку. Если удалить позицию Y из заказа, ошибка остаётся - позиции "без учёта" не могут удовлетворить это правило.
    /// 
    /// Q4: Позиция Y - сопутствующая для X (совпадение)
    ///     Y продана в рубрики A и B. X продана только в A. Должна ли появиться ошибка?
    /// A: Да.
    /// 
    /// Q5: Позиция Z - сопутствующая для X, Y (совпадение)
    ///     Z продана в A, B. X продана в A. Y продана в B. Должна ли появиться ошибка?
    /// A: Нет.
    /// 
    /// TODO: можно вполне выводить в какой именно основной позиции отсутствуют объекты привязки, но в ERM так не делают, и мы не будем
    /// </summary>
    // todo: переименовать PrincipalPositionMustHaveSameBindingObject
    public sealed class LinkedObjectsMissedInPrincipals : ValidationResultAccessorBase
    {
        private const int Match = 1;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedObjectsMissedInPrincipals(IQuery query) : base(query, MessageTypeCode.LinkedObjectsMissedInPrincipals)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderPosition>() on order.Id equals position.OrderId
                select new Dto<Order.OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderAssociatedPosition>() on order.Id equals position.OrderId
                where position.BindingType == Match // небольшой косяк (который есть и в erm) - если сопутствующая удовлетворена мастер-позицией без учёта привязки, то эта проверка выдаст ошибку.
                select new Dto<Order.OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var unsatisfiedPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.WithMatchedBindingObject(orderPositions.DefaultIfEmpty()), (associated, principal) => new { associated, principal })
                                   .GroupBy(x => new
                                   {
                                       // можно включать все поля, какие захотим иметь в выборке, кроме двух: PrincipalPositionId, Source
                                       Start = x.associated.Start,
                                       Category1Id = x.associated.Position.Category1Id,
                                       Category3Id = x.associated.Position.Category3Id,
                                       CauseOrderPositionId = x.associated.Position.CauseOrderPositionId,
                                       CausePackagePositionId = x.associated.Position.CausePackagePositionId,
                                       CauseItemPositionId = x.associated.Position.CauseItemPositionId,
                                       FirmAddressId = x.associated.Position.FirmAddressId,
                                       FirmId = x.associated.FirmId,
                                       OrganizationUnitId = x.associated.OrganizationUnitId,
                                       OrderId = (long?)x.associated.Position.OrderId,
                                   })
                                   .Where(x => x.All(y => y.principal == null))
                                   .Select(grouping => new
                                   {
                                       grouping.Key,

                                       ProjectId = query.For<Period>().Single(x => x.Start == grouping.Key.Start && x.OrganizationUnitId == grouping.Key.OrganizationUnitId).ProjectId,
                                       End = query.For<Period>().Single(x => x.Start == grouping.Key.Start && x.OrganizationUnitId == grouping.Key.OrganizationUnitId).End,
                                   });

            var messages =
                from unsatisfied in unsatisfiedPositions
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(unsatisfied.Key.CauseOrderPositionId,
                                        new Reference<EntityTypeOrder>(unsatisfied.Key.OrderId.Value),
                                        new Reference<EntityTypePosition>(unsatisfied.Key.CausePackagePositionId),
                                        new Reference<EntityTypePosition>(unsatisfied.Key.CauseItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = unsatisfied.Key.Start,
                        PeriodEnd = unsatisfied.End,
                        OrderId = unsatisfied.Key.OrderId,
                        ProjectId = null,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
