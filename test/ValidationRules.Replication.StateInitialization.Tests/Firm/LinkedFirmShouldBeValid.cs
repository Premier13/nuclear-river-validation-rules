﻿using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LinkedFirmShouldBeValid
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedFirmShouldBeValid))
                .Fact(
                    new Facts::Firm { Id = 1, IsClosedForAscertainment = false, IsActive = true, IsDeleted = false, },
                    new Facts::Order { Id = 2, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },

                    new Facts::Firm { Id = 3, IsClosedForAscertainment = true, IsActive = true, IsDeleted = false, },
                    new Facts::Order { Id = 4, FirmId = 3, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },

                    new Facts::Firm { Id = 5, IsClosedForAscertainment = true, IsActive = false, IsDeleted = false, },
                    new Facts::Order { Id = 6, FirmId = 5, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },

                    new Facts::Firm { Id = 7, IsClosedForAscertainment = true, IsActive = false, IsDeleted = true, },
                    new Facts::Order { Id = 8, FirmId = 7, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 })
                .Aggregate(
                    new Order { Id = 2, FirmId = 1, Begin = MonthStart(1), End = MonthStart(2) },

                    new Order { Id = 4, FirmId = 3, Begin = MonthStart(1), End = MonthStart(2) },
                    new Order.InvalidFirm { OrderId = 4, FirmId = 3, State = InvalidFirmState.ClosedForAscertainment },

                    new Order { Id = 6, FirmId = 5, Begin = MonthStart(1), End = MonthStart(2) },
                    new Order.InvalidFirm { OrderId = 6, FirmId = 5, State = InvalidFirmState.ClosedForever },

                    new Order { Id = 8, FirmId = 7, Begin = MonthStart(1), End = MonthStart(2) },
                    new Order.InvalidFirm { OrderId = 8, FirmId = 7, State = InvalidFirmState.Deleted })
                .Message(
                    new Version.ValidationResult
                        {
                            MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmState", (int)InvalidFirmState.ClosedForAscertainment } },
                                    new Reference<EntityTypeFirm>(3),
                                    new Reference<EntityTypeOrder>(4))
                                .ToXDocument(),

                            MessageType = (int)MessageTypeCode.LinkedFirmShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 4,
                        },

                    new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmState", (int)InvalidFirmState.ClosedForever } },
                                    new Reference<EntityTypeFirm>(5),
                                    new Reference<EntityTypeOrder>(6))
                                .ToXDocument(),

                        MessageType = (int)MessageTypeCode.LinkedFirmShouldBeValid,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 6,
                    },

                    new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmState", (int)InvalidFirmState.Deleted } },
                                    new Reference<EntityTypeFirm>(7),
                                    new Reference<EntityTypeOrder>(8))
                                .ToXDocument(),

                        MessageType = (int)MessageTypeCode.LinkedFirmShouldBeValid,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 8,
                    });
    }
}

