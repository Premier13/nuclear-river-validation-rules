﻿using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.PriceRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementCountPerCategoryShouldBeLimited
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementCountPerCategoryShouldBeLimited))
                .Aggregate(
                    // Одобренный заказ с продажей на три месяца
                    new Order { Id = 1 },
                    new Order.OrderCategoryPosition { OrderId = 1, CategoryId = 3 },
                    new Order.OrderPeriod { OrderId = 1, Begin = MonthStart(1), End = MonthStart(4), Scope = 0 },

                    // Другой одобренный заказ с продажей (пересекается только в одном месяце)
                    new Order { Id = 2 },
                    new Order.OrderCategoryPosition { OrderId = 2, CategoryId = 3 },
                    new Order.OrderPeriod { OrderId = 2, Begin = MonthStart(3), End = MonthStart(6), Scope = 0 },

                    // Заказ "на утверждении", размещается, когда есть две одобренных продажи и получает ошибку
                    new Order { Id = 3 },
                    new Order.OrderCategoryPosition { OrderId = 3, CategoryId = 3 },
                    new Order.OrderPeriod { OrderId = 3, Begin = MonthStart(3), End = MonthStart(5), Scope = -1 },

                    // Заказ "на оформлении", размещается, когда есть одна одобренная продажа и один заказ на оформлении и тоже получает ошибку
                    new Order { Id = 4 },
                    new Order.OrderCategoryPosition { OrderId = 4, CategoryId = 3 },
                    new Order.OrderPeriod { OrderId = 4, Begin = MonthStart(4), End = MonthStart(6), Scope = 4 },

                    new Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Period { Start = MonthStart(4), End = MonthStart(5) },
                    new Period { Start = MonthStart(5), End = MonthStart(6) },
                    new Period { Start = MonthStart(6), End = MonthStart(7) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "max", 2 }, { "count", 3 } },
                                new Reference<EntityTypeCategory>(3),
                                new Reference<EntityTypeProject>(0)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                            PeriodStart = MonthStart(3),
                            PeriodEnd = MonthStart(4),
                            OrderId = 3,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "max", 2 }, { "count", 3 } },
                                new Reference<EntityTypeCategory>(3),
                                new Reference<EntityTypeProject>(0)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                            PeriodStart = MonthStart(4),
                            PeriodEnd = MonthStart(5),
                            OrderId = 4,
                        });
    }
}
