﻿using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.PriceRules;
using NuClear.ValidationRules.Storage.Model.Messages;

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
    public sealed class FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject : ValidationResultAccessorBase
    {
        public FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject(IQuery query) : base(query, MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var firmPositions = query.For<Firm.FirmPosition>();
            var firmAssociatedPositions = query.For<Firm.FirmAssociatedPosition>();

            var errors =
                firmPositions
                     .Select(Specs.Join.Aggs.WithPrincipalPositions(firmAssociatedPositions, firmPositions))
                     .Where(dto => !dto.Principals.Any(x => x.IsBindingObjectConditionSatisfied) && dto.Principals.Any(x => x.RequiredMatch))
                     .Select(dto => dto.Associated);

            var messages =
                from error in errors
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(error.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.OrderId),
                                        new Reference<EntityTypePosition>(error.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.ItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = error.Start,
                        PeriodEnd = error.End,
                        OrderId = error.OrderId,
                    };

            return messages;
        }
    }
}
