﻿namespace NuClear.ValidationRules.Replication
{
    public enum MessageTypeCode
    {
        MaximumAdvertisementAmount = 1,
        MinimalAdvertisementRestrictionShouldBeSpecified = 2,
        OrderPositionsShouldCorrespontToActualPrice = 3,
        OrderPositionCorrespontToInactivePosition = 4,
        OrderPositionShouldCorrespontToActualPrice = 5,
        MinimumAdvertisementAmount = 6,
        AssociatedPositionsGroupCount = 7,
        DeniedPositionsCheck = 8,
        AssociatedPositionWithoutPrincipal = 9,
        LinkedObjectsMissedInPrincipals = 10,
        ConflictingPrincipalPosition = 11,
        SatisfiedPrincipalPositionDifferentOrder = 15,
        AdvertisementCountPerThemeShouldBeLimited = 16,
        AdvertisementCountPerCategoryShouldBeLimited = 17,

        AccountShouldExist = 12,
        LockShouldNotExist = 13,
        AccountBalanceShouldBePositive = 14,

        OrderBeginDistrubutionShouldBeFirstDayOfMonth = 18,
        OrderEndDistrubutionShouldBeLastSecondOfMonth = 19,
        LegalPersonProfileBargainShouldNotBeExpired = 20,
        LegalPersonProfileWarrantyShouldNotBeExpired = 21,
        BillsPeriodShouldMatchOrder = 22,
        OrderShouldNotBeSignedBeforeBargain = 23,
        LegalPersonShouldHaveAtLeastOneProfile = 24,
        OrderShouldHaveAtLeastOnePosition = 25,
        OrderScanShouldPresent = 26,
        BargainScanShouldPresent = 27,
        OrderRequiredFieldsShouldBeSpecified = 28,
        LinkedFirmAddressShouldBeValid = 29,
        LinkedCategoryFirmAddressShouldBeValid = 30,
        LinkedCategoryShouldBelongToFirm = 32,
        LinkedCategoryAsterixMayBelongToFirm = 33,
        LinkedCategoryShouldBeActive = 34,
        LinkedFirmShouldBeValid = 35,
        BillsSumShouldMatchOrder = 36,
        BillsShouldBeCreated = 37,

        FirmAndOrderShouldBelongTheSameOrganizationUnit = 38,
        FirmShouldHaveLimitedCategoryCount = 39,
        FirmWithSpecialCategoryShouldHaveSpecialPurchases = 40,

        OrderPositionAdvertisementMustHaveAdvertisement = 41,
        OrderPositionAdvertisementMustBeCreated = 42,
        OrderPositionMustNotReferenceDeletedAdvertisement = 43,
        AdvertisementMustBelongToFirm = 44,
        OrderMustNotContainDummyAdvertisement = 45,
        OrderMustHaveAdvertisement = 46,
        AdvertisementElementMustPassReview = 47,
        WhiteListAdvertisementMustPresent = 48,
        WhiteListAdvertisementMayPresent = 49,

        ProjectMustContainCostPerClickMinimumRestriction = 50,
        OrderMustUseCategoriesOnlyAvailableInProject = 51,
        OrderMustNotIncludeReleasedPeriod = 52,
        OrderPositionCostPerClickMustNotBeLessMinimum = 53,
        FirmAddressMustBeLocatedOnTheMap = 54,

        ThemeCategoryMustBeActiveAndNotDeleted = 55,
        ThemePeriodMustContainOrderPeriod = 56,
        DefaultThemeMustHaveOnlySelfAds = 57,
        DefaultThemeMustBeExactlyOne = 58,

        OrderPositionCostPerClickMustBeSpecified = 59,
        OrderPositionSalesModelMustMatchCategorySalesModel = 60,

        FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions = 61,

        OrderMustHaveActiveDeal = 62,
        OrderMustHaveActiveLegalEntities = 63,
        AdvantageousPurchasesBannerMustBeSoldInTheSameCategory = 64,
    }
}
