﻿using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.AccountRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountShouldExistNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountShouldExistNegative))
                .Fact(
                    new Facts::Order { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::Project())
                .Aggregate(
                    new Order { Id = 1, AccountId = null, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AccountShouldExist,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayMar,
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountShouldExistPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountShouldExistPositive))
                .Fact(
                    new Facts::Order { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::Account { Id = 4, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3 },
                    new Facts::Project())
                .Aggregate(
                    new Order { Id = 1, AccountId = 4, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar })
                .Message();
    }
}
