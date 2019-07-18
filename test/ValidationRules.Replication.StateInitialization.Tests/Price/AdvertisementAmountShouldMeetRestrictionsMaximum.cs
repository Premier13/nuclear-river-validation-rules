﻿using System;
using System.Collections.Generic;

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
        private static ArrangeMetadataElement AdvertisementAmountShouldMeetRestrictionsMaximum
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementAmountShouldMeetRestrictionsMaximum))
                .Aggregate(
                    new Ruleset.AdvertisementAmountRestriction { Begin = MonthStart(1), End = DateTime.MaxValue,
                        CategoryCode = 1, Max = 2 },

                    new Order { Id = 1 },
                    new Order.AmountControlledPosition { OrderId = 1, CategoryCode = 1 },
                    new Order.OrderPeriod { OrderId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                    new Order { Id = 2 },
                    new Order.AmountControlledPosition { OrderId = 2, CategoryCode = 1 },
                    new Order.OrderPeriod { OrderId = 2, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                    new Order { Id = 3 },
                    new Order.AmountControlledPosition { OrderId = 3, CategoryCode = 1 },
                    new Order.OrderPeriod { OrderId = 3, Begin = MonthStart(1), End = MonthStart(2), Scope = -1 },

                    new Order { Id = 4 },
                    new Order.AmountControlledPosition { OrderId = 4, CategoryCode = 1 },
                    new Order.OrderPeriod { OrderId = 4, Begin = MonthStart(1), End = MonthStart(2), Scope = 4 },

                    new Order { Id = 5 },
                    new Order.AmountControlledPosition { OrderId = 5, CategoryCode = 1 },
                    new Order.OrderPeriod { OrderId = 5, Begin = MonthStart(2), End = MonthStart(3), Scope = 5 },

                    new Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Period { Start = MonthStart(2), End = MonthStart(3) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 3 }, { "start", MonthStart(1) }, { "end", MonthStart(2) } },
                                    new Reference<EntityTypeOrder>(3),
                                    new Reference<EntityTypeNomenclatureCategory>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 4 }, { "start", MonthStart(1) }, { "end", MonthStart(2) } },
                                    new Reference<EntityTypeOrder>(4),
                                    new Reference<EntityTypeNomenclatureCategory>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 3 }, { "start", MonthStart(2) }, { "end", MonthStart(3) } },
                                    new Reference<EntityTypeOrder>(5),
                                    new Reference<EntityTypeNomenclatureCategory>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 5,
                        });
    }
}
