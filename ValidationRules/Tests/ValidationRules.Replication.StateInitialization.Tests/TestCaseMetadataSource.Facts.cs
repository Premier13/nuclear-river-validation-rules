﻿using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPosition))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPosition { Id = 2, IsActive = true, IsDeleted = true })
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPosition))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = true, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                )
            .Fact(
                new Facts::AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPositionsGroup))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPositionsGroup { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPositionsGroup))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = true, PricePositionId = 1 }
                )
            .Fact(
                new Facts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredCategory))
            .Erm(
                new Erm::Category { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::Category { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedCategory))
            .Erm(
                new Erm::Category { Id = 1, IsActive = true, IsDeleted = false, ParentId = 1 }
                )
            .Fact(
                new Facts::Category { Id = 1, ParentId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredDeniedPosition))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::DeniedPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedDeniedPosition))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = true, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                )
            .Fact(
                new Facts::DeniedPosition { Id = 1, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredRuleset
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredRuleset))
            .Erm(
                new Erm::Ruleset { Id = 1, IsDeleted = true },
                new Erm::RulesetRule { RulesetId = 1},
                new Erm::RulesetRule { RulesetId = 0 }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Ruleset
        => ArrangeMetadataElement.Config
            .Name(nameof(Ruleset))
            .Erm(
                new Erm::Ruleset { Id = 1, Priority = 1 },
                new Erm::RulesetRule { RulesetId = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new Erm::Ruleset { Id = 2, Priority = 2 },
                new Erm::RulesetRule { RulesetId = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                )
            .Fact(
                new Facts::RulesetRule { Id = 1, Priority = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new Facts::RulesetRule { Id = 2, Priority = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrder))
            .Erm(
                new Erm::Order { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::Order { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrder))
            .Erm(
                new Erm::Order { Id = 1, IsActive = true, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndDistributionDateFact = DateTime.Parse("2012-01-31T23:59:59"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerCode = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8 },
                new Erm::Project { Id = 3, OrganizationUnitId = 2, IsActive = true },
                new Erm::Project { Id = 8, OrganizationUnitId = 7, IsActive = true }
                )
            .Fact(
                new Facts::Order { Id = 1, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestProjectId = 3, EndDistributionDateFact = DateTime.Parse("2012-02-01"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerId = 6, SourceProjectId = 8, WorkflowStepId = 8 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrderPosition))
            .Erm(
                new Erm::OrderPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::OrderPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPosition))
            .Erm(
                new Erm::OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 }
                )
            .Fact(
                new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPositionAdvertisement
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPositionAdvertisement))
            .Erm(
                new Erm::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                )
            .Fact(
                new Facts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrganizationUnit))
            .Erm(
                new Erm::OrganizationUnit { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::OrganizationUnit { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrganizationUnit))
            .Erm(
                new Erm::OrganizationUnit { Id = 1, IsActive = true }
                )
            .Fact(
                new Facts::OrganizationUnit { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPosition))
            .Erm(
                new Erm::Position { Id = 2, IsDeleted = true }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPosition))
            .Erm(
                new Erm::Position { Id = 33, BindingObjectTypeEnum = 33 },
                new Erm::Position { Id = 34, BindingObjectTypeEnum = 34 },
                new Erm::Position { Id = 1, BindingObjectTypeEnum = 1 },

                new Erm::Position { Id = 6, BindingObjectTypeEnum = 6 },
                new Erm::Position { Id = 35, BindingObjectTypeEnum = 35 },

                new Erm::Position { Id = 7, BindingObjectTypeEnum = 7 },
                new Erm::Position { Id = 8, BindingObjectTypeEnum = 8 },

                new Erm::Position { Id = 36, BindingObjectTypeEnum = 36 },
                new Erm::Position { Id = 37, BindingObjectTypeEnum = 37 },

                new Erm::Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                )
            .Fact(
                new Facts::Position { Id = 33, CompareMode = 1 },
                new Facts::Position { Id = 34, CompareMode = 1 },
                new Facts::Position { Id = 1, CompareMode = 1 },

                new Facts::Position { Id = 6, CompareMode = 2 },
                new Facts::Position { Id = 35, CompareMode = 2 },

                new Facts::Position { Id = 7, CompareMode = 3 },
                new Facts::Position { Id = 8, CompareMode = 3 },

                new Facts::Position { Id = 36, CompareMode = 4 },
                new Facts::Position { Id = 37, CompareMode = 4 },

                new Facts::Position { Id = 999, CompareMode = 0, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPricePosition))
            .Erm(
                new Erm::PricePosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::PricePosition { Id = 2, IsActive = false, IsDeleted = true },
                new Erm::PricePosition { Id = 3, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPricePosition))
            .Erm(
                new Erm::PricePosition { Id = 1, IsActive = true, IsDeleted = false }
                )
            .Fact(
                new Facts::PricePosition { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredProject
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredProject))
            .Erm(
                new Erm::Project { Id = 1, IsActive = false, OrganizationUnitId = 1 },
                new Erm::Project { Id = 2, IsActive = true, OrganizationUnitId = null }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedProject
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedProject))
            .Erm(
                new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 }
                )
            .Fact(
                new Facts::Project { Id = 1, OrganizationUnitId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPrice
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPrice))
            .Erm(
                new Erm::Price { Id = 1, IsActive = false, IsDeleted = false, IsPublished = true },
                new Erm::Price { Id = 2, IsActive = true, IsDeleted = true, IsPublished = true },
                new Erm::Price { Id = 3, IsActive = true, IsDeleted = false, IsPublished = false }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPrice
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPrice))
            .Erm(
                new Erm::Price { Id = 2, IsActive = true, IsDeleted = false, IsPublished = true, OrganizationUnitId = 1 },
                new Erm::Project { OrganizationUnitId = 1, IsActive = true }
                )
            .Fact(
                new Facts::Price { Id = 2 },
                new Facts::Project { OrganizationUnitId = 1 }
                );

    }
}
