﻿using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderBeginDistrubutionShouldBeFirstDayOfMonth
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderBeginDistrubutionShouldBeFirstDayOfMonth))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1).AddDays(1), EndDistributionPlan = MonthStart(2) })
                .Aggregate(
                    new Order { Id = 1, BeginDistribution = MonthStart(1).AddDays(1), EndDistributionPlan = MonthStart(2) },
                    new Order.InvalidBeginDistributionDate { OrderId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeOrder>(1))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth,
                            PeriodStart = MonthStart(1).AddDays(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
