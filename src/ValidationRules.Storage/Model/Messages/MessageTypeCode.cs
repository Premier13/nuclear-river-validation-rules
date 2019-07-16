﻿namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public enum MessageTypeCode
    {
        AdvertisementAmountShouldMeetMaximumRestrictions = 1,
        OrderMustHaveActualPrice = 3,
        OrderPositionCorrespontToInactivePosition = 4,
        OrderPositionMayCorrespontToActualPrice = 5,
        OrderPositionMustCorrespontToActualPrice = 31,
        AdvertisementAmountShouldMeetMinimumRestrictions = 6,
        AdvertisementAmountShouldMeetMinimumRestrictionsMass = 70,
        FirmPositionMustNotHaveDeniedPositions = 8,
        FirmAssociatedPositionMustHavePrincipal = 9,
        FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject = 10,
        FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject = 11,
        FirmAssociatedPositionShouldNotStayAlone = 15,
        AdvertisementCountPerThemeShouldBeLimited = 16,
        AdvertisementCountPerCategoryShouldBeLimited = 17,

        AccountShouldExist = 12,
        AccountBalanceShouldBePositive = 14,

        LegalPersonProfileBargainShouldNotBeExpired = 20,
        LegalPersonProfileWarrantyShouldNotBeExpired = 21,
        OrderShouldNotBeSignedBeforeBargain = 23,
        LegalPersonShouldHaveAtLeastOneProfile = 24,
        OrderShouldHaveAtLeastOnePosition = 25,
        OrderScanShouldPresent = 26,
        BargainScanShouldPresent = 27,
        OrderRequiredFieldsShouldBeSpecified = 28,
        LinkedFirmAddressShouldBeValid = 29,
        LinkedCategoryFirmAddressShouldBeValid = 30,
        LinkedCategoryShouldBelongToFirm = 32,
        LinkedCategoryAsteriskMayBelongToFirm = 33,
        LinkedCategoryShouldBeActive = 34,
        LinkedFirmShouldBeValid = 35,
        BillsSumShouldMatchOrder = 36,
        BillsShouldBeCreated = 37,

        FirmAndOrderShouldBelongTheSameOrganizationUnit = 38,
        FirmShouldHaveLimitedCategoryCount = 39,

        OrderPositionAdvertisementMustHaveAdvertisement = 41,
        OrderPositionAdvertisementMustBeCreated = 42,
        OrderPositionAdvertisementMustHaveOptionalAdvertisement = 43,
        AdvertisementMustBelongToFirm = 44,
        AdvertisementMustPassReview = 47,
        AdvertisementShouldNotHaveComments = 48,
        OptionalAdvertisementMustPassReview = 49,

        ProjectMustContainCostPerClickMinimumRestriction = 13,
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

        OrderMustHaveActiveDeal = 62,
        OrderMustHaveActiveLegalEntities = 63,

        FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement = 71,
        FirmAddressShouldNotHaveMultiplePartnerAdvertisement = 72,

        PartnerAdvertisementShouldNotBeSoldToAdvertiser = 74,
        PartnerAdvertisementMustNotCauseProblemsToTheAdvertiser = 75,

        AmsMessagesShouldBeNew = 80,

        PoiAmountForEntranceShouldMeetMaximumRestrictions = 100,

        AtLeastOneLinkedPartnerFirmAddressShouldBeValid = 101
    }
}
