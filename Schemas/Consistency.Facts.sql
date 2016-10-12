﻿if not exists (select * from sys.schemas where name = 'ConsistencyFacts') exec('create schema ConsistencyFacts')
go

if object_id('ConsistencyFacts.Bargain') is not null drop table ConsistencyFacts.Bargain
if object_id('ConsistencyFacts.BargainScanFile') is not null drop table ConsistencyFacts.BargainScanFile
if object_id('ConsistencyFacts.Bill') is not null drop table ConsistencyFacts.Bill
if object_id('ConsistencyFacts.Firm') is not null drop table ConsistencyFacts.Firm
if object_id('ConsistencyFacts.Category') is not null drop table ConsistencyFacts.Category
if object_id('ConsistencyFacts.CategoryFirmAddress') is not null drop table ConsistencyFacts.CategoryFirmAddress
if object_id('ConsistencyFacts.FirmAddress') is not null drop table ConsistencyFacts.FirmAddress
if object_id('ConsistencyFacts.LegalPersonProfile') is not null drop table ConsistencyFacts.LegalPersonProfile
if object_id('ConsistencyFacts.[Order]') is not null drop table ConsistencyFacts.[Order]
if object_id('ConsistencyFacts.OrderPosition') is not null drop table ConsistencyFacts.OrderPosition
if object_id('ConsistencyFacts.OrderPositionAdvertisement') is not null drop table ConsistencyFacts.OrderPositionAdvertisement
if object_id('ConsistencyFacts.OrderScanFile') is not null drop table ConsistencyFacts.OrderScanFile
if object_id('ConsistencyFacts.Position') is not null drop table ConsistencyFacts.Position
if object_id('ConsistencyFacts.Project') is not null drop table ConsistencyFacts.Project
if object_id('ConsistencyFacts.ReleaseWithdrawal') is not null drop table ConsistencyFacts.ReleaseWithdrawal
go

create table ConsistencyFacts.Bargain (
    Id bigint not null,
    SignupDate datetime2(2) not null,
    constraint PK_Bargain primary key (Id)
)
go

create table ConsistencyFacts.BargainScanFile (
    Id bigint not null,
    BargainId bigint not null,
    constraint PK_BargainScanFile primary key (Id)
)
go

create table ConsistencyFacts.Bill (
    Id bigint not null,
    OrderId bigint not null,
    PayablePlan decimal(19,4) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    constraint PK_Bill primary key (Id)
)
go

create table ConsistencyFacts.Category (
    Id bigint not null,
    Name nvarchar(128) not null,
    IsActiveNotDeleted bit not null,
    constraint PK_Category primary key (Id)
)
go

create table ConsistencyFacts.Firm (
    Id bigint not null,
    IsClosedForAscertainment bit not null,
    IsActive bit not null,
    IsDeleted bit not null,
    Name nvarchar(250) not null,
    constraint PK_Firm primary key (Id)
)
go

create table ConsistencyFacts.FirmAddress (
    Id bigint not null,
    FirmId bigint not null,
    Name nvarchar(512) not null,
    IsClosedForAscertainment bit not null,
    IsActive bit not null,
    IsDeleted bit not null,
    constraint PK_FirmAddress primary key (Id)
)
go

create table ConsistencyFacts.CategoryFirmAddress (
    Id bigint not null,
    FirmAddressId bigint not null,
    CategoryId bigint not null,
    constraint PK_CategoryFirmAddress primary key (Id, CategoryId)
)
go

create table ConsistencyFacts.LegalPersonProfile (
    Id bigint not null,
    LegalPersonId bigint not null,
    BargainEndDate datetime2(2) null,
    WarrantyEndDate datetime2(2) null,
    Name nvarchar(256) not null,
    constraint PK_LegalPersonProfile primary key (Id)
)
go

create table ConsistencyFacts.[Order](
    Id bigint not null,
    FirmId bigint not null,
    DestOrganizationUnitId bigint not null,
    LegalPersonId bigint null,
    LegalPersonProfileId bigint null,
    BranchOfficeOrganizationUnitId bigint null,
    InspectorId bigint null,
    CurrencyId bigint null,
    BargainId bigint null,
    WorkflowStep int not null,
    IsFreeOfCharge bit not null,

    SignupDate datetime2(2) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
    ReleaseCountPlan int not null,
    Number nvarchar(64) not null,
    constraint PK_Order primary key (Id)
)
go

create table ConsistencyFacts.OrderPosition (
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go

create table ConsistencyFacts.OrderPositionAdvertisement (
    Id bigint not null,
    OrderPositionId bigint not null,
    FirmAddressId bigint null,
    CategoryId bigint null,
    PositionId bigint null,
    constraint PK_OrderPositionAdvertisement primary key (Id)
)
go

create table ConsistencyFacts.OrderScanFile (
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderScanFile primary key (Id)
)
go

create table ConsistencyFacts.Position (
    Id bigint not null,
    BindingObjectType int not null,
    Name nvarchar(256) not null,
    constraint PK_Position primary key (Id)
)
go

create table ConsistencyFacts.Project (
    Id bigint not null,
    OrganizationUnitId bigint not null,
    constraint PK_Project primary key (Id)
)
go

create table ConsistencyFacts.ReleaseWithdrawal (
    Id bigint not null,
    OrderPositionId bigint not null,
    Amount decimal(19,4) not null,
    constraint PK_ReleaseWithdrawal primary key (Id)
)
go

CREATE NONCLUSTERED INDEX IX_Order_LegalPersonId_SignupDate
ON [ConsistencyFacts].[Order] ([LegalPersonId],[SignupDate])
INCLUDE ([Id])
GO

CREATE NONCLUSTERED INDEX IX_FirmAddress_Id
ON [ConsistencyFacts].[FirmAddress] ([Id])

GO

CREATE NONCLUSTERED INDEX IX_OrderScanFile_OrderId
ON [ConsistencyFacts].[OrderScanFile] ([OrderId])
INCLUDE ([Id])
GO

CREATE NONCLUSTERED INDEX IX_Firm_Id
ON [ConsistencyFacts].[Firm] ([Id])
INCLUDE ([IsClosedForAscertainment],[IsActive],[IsDeleted],[Name])
GO

CREATE NONCLUSTERED INDEX IX_LegalPersonProfile_LegalPersonId
ON [ConsistencyFacts].[LegalPersonProfile] ([LegalPersonId])
INCLUDE ([Id])
GO

CREATE NONCLUSTERED INDEX IX_ReleaseWithdrawal_OrderPositionId
ON [ConsistencyFacts].[ReleaseWithdrawal] ([OrderPositionId])
INCLUDE ([Amount])
GO

CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_OrderPositionId
ON [ConsistencyFacts].[OrderPositionAdvertisement] ([OrderPositionId])
INCLUDE ([FirmAddressId],[PositionId])
GO

CREATE NONCLUSTERED INDEX IX_OrderPosition_OrderId
ON [ConsistencyFacts].[OrderPosition] ([OrderId])
INCLUDE ([Id])
GO

CREATE NONCLUSTERED INDEX IX_Order_BargainId
ON [ConsistencyFacts].[Order] ([BargainId])
INCLUDE ([Id])
GO

CREATE NONCLUSTERED INDEX IX_Order_DestOrganizationUnitId
ON [ConsistencyFacts].[Order] ([DestOrganizationUnitId])
INCLUDE ([Id],[BeginDistribution],[EndDistributionFact],[EndDistributionPlan],[Number])
GO

CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_FirmAddressId_CategoryId
ON [ConsistencyFacts].[OrderPositionAdvertisement] ([FirmAddressId],[CategoryId])
INCLUDE ([OrderPositionId],[PositionId])
GO

CREATE NONCLUSTERED INDEX IX_Order_BargainId_SignupDate
ON [ConsistencyFacts].[Order] ([BargainId],[SignupDate])
INCLUDE ([Id])
GO