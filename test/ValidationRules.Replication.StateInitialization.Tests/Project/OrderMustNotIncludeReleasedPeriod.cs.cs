﻿using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ProjectRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustNotIncludeReleasedPeriodPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotIncludeReleasedPeriodPositive))
                .Fact(
                    new Facts::Order { Id = 1, AgileDistributionStartDate = MonthStart(1), AgileDistributionEndPlanDate = MonthStart(3), WorkflowStep = 1 },
                    new Facts::ReleaseInfo { PeriodEndDate = MonthStart(2) })
                .Aggregate(
                    new Order { Id = 1, Start = MonthStart(1), End = MonthStart(3), IsDraft = true },
                    new Project.NextRelease { Date = MonthStart(2) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustNotIncludeReleasedPeriodNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotIncludeReleasedPeriodNegative))
                .Fact(
                    new Facts::Order { Id = 1, AgileDistributionStartDate = MonthStart(2), AgileDistributionEndPlanDate = MonthStart(3), WorkflowStep = 1 },
                    new Facts::Order { Id = 2, AgileDistributionStartDate = MonthStart(1), AgileDistributionEndPlanDate = MonthStart(3), WorkflowStep = 5 },
                    new Facts::ReleaseInfo { PeriodEndDate = MonthStart(2) })
                .Aggregate(
                    new Order { Id = 1, Start = MonthStart(2), End = MonthStart(3), IsDraft = true },
                    new Order { Id = 2, Start = MonthStart(1), End = MonthStart(3), IsDraft = false },
                    new Project.NextRelease { Date = MonthStart(2) })
                .Message();
    }
}
