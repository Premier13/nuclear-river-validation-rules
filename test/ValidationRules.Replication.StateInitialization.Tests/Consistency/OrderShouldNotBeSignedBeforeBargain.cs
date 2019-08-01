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
        private static ArrangeMetadataElement OrderShouldNotBeSignedBeforeBargain
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderShouldNotBeSignedBeforeBargain))
                .Fact(
                    new Facts::Order { Id = 1, BargainId = 1, SignupDate = MonthStart(1), AgileDistributionStartDate = MonthStart(1), AgileDistributionEndPlanDate = MonthStart(2) },
                    new Facts::Bargain { Id = 1, SignupDate = MonthStart(1).AddDays(1) })
                .Aggregate(
                    new Order { Id = 1, Start = MonthStart(1), End = MonthStart(2) },
                    new Order.BargainSignedLaterThanOrder { OrderId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeOrder>(1))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderShouldNotBeSignedBeforeBargain,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
