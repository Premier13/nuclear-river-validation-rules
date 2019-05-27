﻿using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.ThemeRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DefaultThemeMustHaveOnlySelfAdsPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DefaultThemeMustHaveOnlySelfAdsPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, IsSelfAds = false },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Aggregate(
                    new Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb, IsSelfAds = false },
                    new Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Reference<EntityTypeTheme>(5)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustHaveOnlySelfAds,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 1,
                        }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DefaultThemeMustHaveOnlySelfAdsNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DefaultThemeMustHaveOnlySelfAdsNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, IsSelfAds = false },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = false }
                )
                .Aggregate(
                    new Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb, IsSelfAds = false },
                    new Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = false }
                )
                .Message(
                );
    }
}
