using NuClear.DataTest.Metamodel.Dsl;
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
                    // лиц. счёт не привязан к договору
                    new Facts::Order { Id = 1, AgileDistributionStartDate = FirstDayJan, AgileDistributionEndFactDate = FirstDayMar },
                    new Facts::OrderWorkflow { Id = 1, Step = 4 },
                    new Facts::OrderConsistency { Id = 1, BargainId = 1 },
                    new Facts::Bargain { Id = 1, AccountId = null},

                    // договор не привязан к заказу
                    new Facts::Order { Id = 2, AgileDistributionStartDate = FirstDayJan, AgileDistributionEndFactDate = FirstDayMar },
                    new Facts::OrderWorkflow { Id = 2, Step = 4 },
                    new Facts::OrderConsistency { Id = 2, BargainId = null })
                .Aggregate(
                    new Order { Id = 1, AccountId = null, Start = FirstDayJan, End = FirstDayMar },
                    new Order { Id = 2, AccountId = null, Start = FirstDayJan, End = FirstDayMar })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AccountShouldExist,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayMar,
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(2)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.AccountShouldExist,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayMar,
                        OrderId = 2,
                    });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountShouldExistPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountShouldExistPositive))
                .Fact(
                    new Facts::Order { Id = 1, AgileDistributionStartDate = FirstDayJan, AgileDistributionEndFactDate = FirstDayMar },
                    new Facts::OrderWorkflow { Id = 1, Step = 4 },
                    new Facts::OrderConsistency { Id = 1, BargainId = 2 },
                    new Facts::Bargain { Id = 2, AccountId = 3},
                    new Facts::Account { Id = 3 })
                .Aggregate(
                    new Order { Id = 1, AccountId = 3, Start = FirstDayJan, End = FirstDayMar })
                .Message();
    }
}
