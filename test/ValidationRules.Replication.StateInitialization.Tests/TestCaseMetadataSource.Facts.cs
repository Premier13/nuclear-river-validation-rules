﻿using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly DateTime FirstDayJan = DateTime.Parse("2012-01-01");
        private static readonly DateTime FirstDayFeb = DateTime.Parse("2012-02-01");
        private static readonly DateTime FirstDayMar = DateTime.Parse("2012-03-01");
        private static readonly DateTime FirstDayApr = DateTime.Parse("2012-04-01");
        private static readonly DateTime FirstDayMay = DateTime.Parse("2012-05-01");
        private static readonly DateTime LastSecondJan = DateTime.Parse("2012-01-31T23:59:59");
        private static readonly DateTime LastSecondMar = DateTime.Parse("2012-03-31T23:59:59");
        private static readonly DateTime LastSecondApr = DateTime.Parse("2012-04-30T23:59:59");

        private static DateTime MonthStart(int i) => DateTime.Parse("2012-01-01").AddMonths(i - 1);

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement UnlimitedOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(UnlimitedOrder))
            .Erm(
                new Erm::UnlimitedOrder { OrderId = 1, IsActive = true, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2).AddSeconds(-1) },
                new Erm::UnlimitedOrder { OrderId = 2, IsActive = false, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2).AddSeconds(-1) })
            .Fact(
                new UnlimitedOrder { OrderId = 1, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2) });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeOrganizationUnitFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeOrganizationUnitFacts))
            .Erm(
                new Erm::ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 2, ThemeId = 2, OrganizationUnitId = 3, IsActive = false, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 3, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeCategoryFacts))
            .Erm(
                new Erm::ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3, IsDeleted = false },
                new Erm::ThemeCategory { Id = 2, ThemeId = 2, CategoryId = 3, IsDeleted = true })
            .Fact(
                new ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement NomenclatureCategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(NomenclatureCategoryFacts))
            .Erm(
                 new Erm::NomenclatureCategory { Id = 1, Name = "one" },
                 new Erm::NomenclatureCategory { Id = 2, Name = "two" })
            .Fact(
                new NomenclatureCategory { Id = 1 },
                new NomenclatureCategory { Id = 2 },
                new EntityName { EntityType = EntityTypeIds.NomenclatureCategory, Id = 1, Name = "one" },
                new EntityName { EntityType = EntityTypeIds.NomenclatureCategory, Id = 2, Name = "two" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SalesModelCategoryRestrictions
        => ArrangeMetadataElement.Config
            .Name(nameof(SalesModelCategoryRestrictions))
            .Erm(
                new Erm::SalesModelCategoryRestriction { ProjectId = 1, BeginningDate = MonthStart(1), CategoryId = 1, SalesModel = 2 })
            .Fact(
                new SalesModelCategoryRestriction { ProjectId = 1, Start = MonthStart(1), CategoryId = 1, SalesModel = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PositionChildFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(PositionChildFacts))
            .Erm(
                new Erm::PositionChild {MasterPositionId = 1, ChildPositionId = 1 })
            .Fact(
                new PositionChild { MasterPositionId = 1, ChildPositionId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BillFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BillFacts))
            .Erm(
                new Erm::Order { Id = 2, IsActive = true },
                new Erm::Bill { Id = 1, IsActive = true, IsDeleted = false, BillType = 1, OrderId = 2, PayablePlan = 123 })
            .Fact(
                new Bill { Id = 1, OrderId = 2, PayablePlan = 123 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFacts))
            .Erm(
                new Erm::Bargain { Id = 1, IsActive = true, IsDeleted = false })
            .Fact(
                new Bargain { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFileFacts))
            .Erm(
                new Erm::OrderFile { Id = 1, OrderId = 1, IsActive = true, IsDeleted = false, FileKind = 8 })
            .Fact(
                new OrderScanFile { Id = 1, OrderId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFileFacts))
            .Erm(
                new Erm::BargainFile { Id = 1, IsActive = true, IsDeleted = false, BargainId = 1, FileKind = 12 })
            .Fact(
                new BargainScanFile { Id = 1, BargainId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LegalPersonProfileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(LegalPersonProfileFacts))
            .Erm(
                new Erm::LegalPersonProfile { Id = 1, IsActive = true, IsDeleted = false, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1 },
                new Erm::LegalPersonProfile { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::LegalPersonProfile { Id = 3, IsActive = true, IsDeleted = true },
                new Erm::LegalPersonProfile { Id = 4, IsActive = false, IsDeleted = true })
            .Fact(
                new LegalPersonProfile { Id = 1, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(CategoryFacts))
            .Erm(
                new Erm::Category { Id = 1, IsActive = true, IsDeleted = false, Level = 1 },
                new Erm::Category { Id = 2, IsActive = true, IsDeleted = false, Level = 2, ParentId = 1 },
                new Erm::Category { Id = 3, IsActive = true, IsDeleted = false, Level = 3, ParentId = 2 },
                new Erm::Category { Id = 4, IsActive = false, IsDeleted = false },
                new Erm::Category { Id = 5, IsActive = true, IsDeleted = true })
            .Fact(

                new Category { Id = 1, IsActiveNotDeleted = true, L1Id = 1 },
                new Category { Id = 2, IsActiveNotDeleted = true, L1Id = 1, L2Id = 2 },
                new Category { Id = 3, IsActiveNotDeleted = true, L1Id = 1, L2Id = 2, L3Id = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFacts))
            .Erm(
                new Erm::Order { Id = 1, IsActive = true, AgileDistributionStartDate = FirstDayJan, AgileDistributionEndFactDate = LastSecondJan, AgileDistributionEndPlanDate = LastSecondMar, DestOrganizationUnitId = 2, FirmId = 5, OwnerCode = 6, WorkflowStepId = 8, CurrencyId = 9, OrderType = 2 },
                new Erm::Order { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::Order { Id = 3, IsActive = true, IsDeleted = true },
                new Erm::Order { Id = 4, IsActive = true, IsDeleted = true },
                new Erm::Project { Id = 3, OrganizationUnitId = 2, IsActive = true })
            .Fact(
                new Order { Id = 1, AgileDistributionStartDate = FirstDayJan, AgileDistributionEndFactDate = LastSecondJan.AddSeconds(1), AgileDistributionEndPlanDate = LastSecondMar.AddSeconds(1), ProjectId = 3, FirmId = 5, IsSelfAds = true },
                new OrderWorkflow {Id = 1, Step = 8},
                new OrderConsistency { Id = 1, HasCurrency = true, IsFreeOfCharge = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionFacts))
                .Erm(
                    new Erm::Order { Id = 1, IsActive = true },
                    new Erm::OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 },
                    new Erm::OrderPosition { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::OrderPosition { Id = 3, IsActive = true, IsDeleted = true })
                .Fact(
                    new OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderItemFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderItemFacts))
                .Erm(
                    new Erm::Order { Id = 1, IsActive = true, IsDeleted = false, WorkflowStepId = 1 },
                    new Erm::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2, IsActive = true, IsDeleted = false },
                    new Erm::OrderPositionAdvertisement { OrderPositionId = 1, PositionId = 3, CategoryId = 1 },
                    new Erm::OrderPositionAdvertisement { OrderPositionId = 1, PositionId = 4, FirmAddressId = 1 },
                    new Erm::PricePosition { Id = 2, PositionId = 5},
                    new Erm::PositionChild { MasterPositionId = 5 })
                .Fact(
                    new OrderItem { OrderId = 1, OrderPositionId = 1, PackagePositionId = 5, ItemPositionId = 5 },
                    new OrderItem { OrderId = 1, OrderPositionId = 1, PackagePositionId = 5, ItemPositionId = 3, CategoryId = 1 },
                    new OrderItem { OrderId = 1, OrderPositionId = 1, PackagePositionId = 5, ItemPositionId = 4, FirmAddressId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionAdvertisementFacts))
                .Erm(
                    new Erm::Order { Id = 2, IsActive = true },
                    new Erm::OrderPosition { Id = 3, OrderId = 2, IsActive = true },
                    new Erm::OrderPositionAdvertisement { CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 })
                .Fact(
                    new OrderPositionAdvertisement {OrderId = 2, OrderPositionId = 3, PositionId = 4, CategoryId = 1, FirmAddressId = 2, });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(PositionFacts))
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

                new Erm::Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsControlledByAmount = true },

                new Erm::Position { Id = 1000, IsDeleted = true })
            .Fact(
                new Position { Id = 33, BindingObjectType = 33 },
                new Position { Id = 34, BindingObjectType = 34 },
                new Position { Id = 1, BindingObjectType = 1 },

                new Position { Id = 6, BindingObjectType = 6 },
                new Position { Id = 35, BindingObjectType = 35 },

                new Position { Id = 7, BindingObjectType = 7 },
                new Position { Id = 8, BindingObjectType = 8 },

                new Position { Id = 36, BindingObjectType = 36 },
                new Position { Id = 37, BindingObjectType = 37 },

                new Position { Id = 999, BindingObjectType = 999, CategoryCode = 1, IsControlledByAmount = true },
                new Position { Id = 1000, IsDeleted = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePositionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(PricePositionFacts))
                .Erm(
                    new Erm::PricePosition { Id = 1, IsActive = false, IsDeleted = false },
                    new Erm::PricePosition { Id = 2, IsActive = false, IsDeleted = true },
                    new Erm::PricePosition { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::PricePosition { Id = 4, IsActive = true, IsDeleted = false, PositionId = 1 })
                .Fact(
                    new PricePosition { Id = 1, IsActiveNotDeleted = false },
                    new PricePosition { Id = 2, IsActiveNotDeleted = false },
                    new PricePosition { Id = 3, IsActiveNotDeleted = false },
                    new PricePosition { Id = 4, PositionId = 1, IsActiveNotDeleted = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectFacts))
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 2, IsActive = false, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 3, IsActive = true, OrganizationUnitId = null })
                .Fact(
                    new Project { Id = 1, OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(PriceFacts))
                .Erm(
                    new Erm::Price { Id = 1, IsDeleted = false, IsPublished = true, ProjectId = 1 },
                    new Erm::Price { Id = 3, IsDeleted = true, IsPublished = true },
                    new Erm::Price { Id = 4, IsDeleted = false, IsPublished = false })
                .Fact(
                    new Price { Id = 1, ProjectId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryOrganizationUnitFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryOrganizationUnitFacts))
                .Erm(
                    new Erm::CategoryOrganizationUnit { Id = 1, IsActive = true, IsDeleted = true, CategoryId = 11, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 2, IsActive = true, IsDeleted = false, CategoryId = 12, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true, CategoryId = 13, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 4, IsActive = false, IsDeleted = false, CategoryId = 14, OrganizationUnitId = 3 })
                .Fact(
                    new CategoryOrganizationUnit { Id = 2, CategoryId = 12, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CostPerClickCategoryRestrictionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CostPerClickCategoryRestrictionFacts))
                .Erm(
                    new Erm::CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, BeginningDate = MonthStart(1), MinCostPerClick = 3 })
                .Fact(
                    new CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, Start = MonthStart(1), MinCostPerClick = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionCostPerClickFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionCostPerClickFacts))
                .Erm(
                    new Erm::OrderPositionCostPerAny { CategoryId = 1, OrderPositionId = 2, Amount = 1, BidIndex = 1 },
                    new Erm::OrderPositionCostPerAny { CategoryId = 1, OrderPositionId = 2, Amount = 2, BidIndex = 2 })
                .Fact(
                    new OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReleaseInfoFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ReleaseInfoFacts))
                .Erm(
                    new Erm::ReleaseInfo { Id = 1, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr, IsActive = true, IsDeleted = false, IsBeta = false, Status = 1 },
                    new Erm::ReleaseInfo { Id = 2, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr, IsActive = true, IsDeleted = false, IsBeta = false, Status = 2 })
                .Fact(
                    new ReleaseInfo { Id = 2, OrganizationUnitId = 2, PeriodEndDate = FirstDayMay });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(AccountFacts))
                .Erm(
                    new Erm::Account { Id = 1, IsArchived = false, Balance = 2 },
                    new Erm::Account { Id = 2, IsArchived = true })
                .Fact(
                    new Account { Id = 1, Balance = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountDetailFacts
            => ArrangeMetadataElement.Config
                                     .Name(nameof(AccountDetailFacts))
                                     .Erm(
                                          new Erm::Account { Id = 1 },
                                          new Erm::AccountDetail { Id = 1, IsDeleted = false, AccountId = 1, OrderId = 1, PeriodStartDate = MonthStart(1) },
                                          new Erm::AccountDetail { Id = 2, IsDeleted = true  },
                                          new Erm::AccountDetail { Id = 3, IsDeleted = false, AccountId = 1, OrderId = null })
                                     .Fact(
                                           new AccountDetail { Id = 1, AccountId = 1, OrderId = 1, PeriodStartDate = MonthStart(1)});

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BranchOfficeFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(BranchOfficeFacts))
                .Erm(
                    new Erm::BranchOffice { Id = 1, IsActive = true, IsDeleted = false },
                    new Erm::BranchOffice { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::BranchOffice { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::BranchOffice { Id = 4, IsActive = false, IsDeleted = true })
                .Fact(
                    new BranchOffice { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BranchOfficeOrganizationUnitFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(BranchOfficeOrganizationUnitFacts))
                .Erm(
                    new Erm::BranchOfficeOrganizationUnit { Id = 1, IsActive = true, IsDeleted = false, BranchOfficeId = 2 },
                    new Erm::BranchOfficeOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::BranchOfficeOrganizationUnit { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::BranchOfficeOrganizationUnit { Id = 4, IsActive = false, IsDeleted = true })
                .Fact(
                    new BranchOfficeOrganizationUnit { Id = 1, BranchOfficeId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DealFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(DealFacts))
                .Erm(
                    new Erm::Deal { Id = 1, IsActive = true, IsDeleted = false },
                    new Erm::Deal { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::Deal { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::Deal { Id = 4, IsActive = false, IsDeleted = true })
                .Fact(
                    new Deal { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LegalPersonFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(LegalPersonFacts))
                .Erm(
                    new Erm::LegalPerson { Id = 1, IsActive = true, IsDeleted = false },
                    new Erm::LegalPerson { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::LegalPerson { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::LegalPerson { Id = 4, IsActive = false, IsDeleted = true })
                .Fact(
                    new LegalPerson { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReleaseWithdrawalFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ReleaseWithdrawalFacts))
                .Erm(
                    new Erm::ReleaseWithdrawal { Id = 1, AmountToWithdraw = 2, OrderPositionId = 3, ReleaseBeginDate = MonthStart(1), ReleaseEndDate = MonthStart(2).AddSeconds(-1) })
                .Fact(
                    new ReleaseWithdrawal { OrderPositionId = 3, Amount = 2, Start = MonthStart(1), End = MonthStart(2) });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ThemeFacts))
                .Erm(
                    new Erm::Theme { Id = 1, IsActive = true, IsDeleted = false, BeginDistribution = MonthStart(1), EndDistribution = MonthStart(2).AddSeconds(-1), IsDefault = true, Name = "TTT" },
                    new Erm::Theme { Id = 2, IsActive = true, IsDeleted = true },
                    new Erm::Theme { Id = 3, IsActive = false, IsDeleted = false },
                    new Erm::Theme { Id = 4, IsActive = false, IsDeleted = true })
                .Fact(
                    new Theme { Id = 1, BeginDistribution = MonthStart(1), EndDistribution = MonthStart(2), IsDefault = true });
    }
}
