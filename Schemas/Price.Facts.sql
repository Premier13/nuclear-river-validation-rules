﻿if not exists (select * from sys.schemas where name = 'PriceFacts') exec('create schema PriceFacts')
go

if object_id('PriceFacts.AssociatedPosition') is not null drop table PriceFacts.AssociatedPosition
if object_id('PriceFacts.AssociatedPositionsGroup') is not null drop table PriceFacts.AssociatedPositionsGroup
if object_id('PriceFacts.DeniedPosition') is not null drop table PriceFacts.DeniedPosition
if object_id('PriceFacts.OrderPositionAdvertisement') is not null drop table PriceFacts.OrderPositionAdvertisement
if object_id('PriceFacts.OrderPosition') is not null drop table PriceFacts.OrderPosition
if object_id('PriceFacts.Order') is not null drop table PriceFacts.[Order]
if object_id('PriceFacts.PricePosition') is not null drop table PriceFacts.PricePosition
if object_id('PriceFacts.PricePositionNotActive') is not null drop table PriceFacts.PricePositionNotActive
if object_id('PriceFacts.Price') is not null drop table PriceFacts.Price
if object_id('PriceFacts.Project') is not null drop table PriceFacts.Project
if object_id('PriceFacts.Position') is not null drop table PriceFacts.Position
if object_id('PriceFacts.Category') is not null drop table PriceFacts.Category
if object_id('PriceFacts.RulesetRule') is not null drop table PriceFacts.RulesetRule
if object_id('PriceFacts.Theme') is not null drop table PriceFacts.Theme
go

create table PriceFacts.Price(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    BeginDate datetime2(2) not null,
    constraint PK_Price primary key (Id)
)
go

create table PriceFacts.Position(
    Id bigint not null,
    CategoryCode bigint not null,
    IsControlledByAmount bit not null,
    IsComposite bit not null,
    Name nvarchar(256) not null,
    constraint PK_Position primary key (Id)
)
create index IX_Position_IsComposite ON PriceFacts.Position (IsComposite)
go

create table PriceFacts.PricePosition(
    Id bigint not null,
    PriceId bigint not null,
    PositionId bigint not null,
    MinAdvertisementAmount int null,
    MaxAdvertisementAmount int null,
    constraint PK_PricePosition primary key (Id)
)
create index IX_PricePosition_PriceId ON PriceFacts.PricePosition (PriceId)
create index IX_PricePosition_PositionId ON PriceFacts.PricePosition (PositionId)
go

create table PriceFacts.PricePositionNotActive(
    Id bigint not null,
    PriceId bigint not null,
    PositionId bigint not null,
    constraint PK_PricePositionNotActive primary key (Id)
)
create index IX_PricePositionNotActive_PriceId ON PriceFacts.PricePositionNotActive (PriceId)
create index IX_PricePositionNotActive_PositionId ON PriceFacts.PricePositionNotActive (PositionId)
go

create table PriceFacts.AssociatedPositionsGroup(
    Id bigint not null,
    PricePositionId bigint not null,
    constraint PK_AssociatedPositionsGroup primary key (Id)
)
create index IX_AssociatedPositionsGroup_PricePositionId ON PriceFacts.AssociatedPositionsGroup (PricePositionId)
go

create table PriceFacts.AssociatedPosition(
    Id bigint not null,
    AssociatedPositionsGroupId bigint not null,
    PositionId bigint not null,
    ObjectBindingType int not null,
    constraint PK_AssociatedPosition primary key (Id)
)
create index IX_AssociatedPosition_AssociatedPositionsGroupId ON PriceFacts.AssociatedPosition (AssociatedPositionsGroupId)
go

create table PriceFacts.DeniedPosition(
    Id bigint not null,
    PositionId bigint not null,
    PositionDeniedId bigint not null,
    ObjectBindingType int not null,
    PriceId bigint not null,
    constraint PK_DeniedPosition primary key (Id)
)
create index IX_DeniedPosition_PriceId ON PriceFacts.DeniedPosition (PriceId)
go

create table PriceFacts.[Order](
    Id bigint not null,
    FirmId bigint not null,
    DestOrganizationUnitId bigint not null,
    SourceOrganizationUnitId bigint not null,
    OwnerId bigint not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDateFact datetime2(2) not null,
    EndDistributionDatePlan datetime2(2) not null,
    BeginReleaseNumber int not null,
    EndReleaseNumberFact int not null,
    EndReleaseNumberPlan int not null,
    WorkflowStepId int not null,
    Number nvarchar(64) not null,
    constraint PK_Order primary key (Id)
)
go

create table PriceFacts.OrderPosition(
    Id bigint not null,
    OrderId bigint not null,
    PricePositionId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
create index IX_OrderPosition_OrderId ON PriceFacts.OrderPosition (OrderId)
create index IX_OrderPosition_PricePositionId ON PriceFacts.OrderPosition (PricePositionId)
go

create table PriceFacts.OrderPositionAdvertisement(
    Id bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    CategoryId bigint null,
    FirmAddressId bigint null,
    ThemeId bigint null,
    constraint PK_OrderPositionAdvertisement primary key (Id)
)
create index IX_OrderPositionAdvertisement_OrderPositionId ON PriceFacts.OrderPositionAdvertisement (OrderPositionId)
create index IX_OrderPositionAdvertisement_PositionId ON PriceFacts.OrderPositionAdvertisement (PositionId)
go

create table PriceFacts.Project(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    Name nvarchar(64) not null,
    constraint PK_Project primary key (Id)
)
go

create table PriceFacts.Theme(
    Id bigint not null,
    Name nvarchar(64) not null,
    constraint PK_Theme primary key (Id)
)
go

create table PriceFacts.Category(
    Id bigint not null,
    Name nvarchar(128) not null,
    L3Id bigint null,
    L2Id bigint null,
    L1Id bigint null,
    constraint PK_Category primary key (Id)
)
go

create table PriceFacts.RulesetRule(
    Id bigint not null,
    RuleType int not null,
    DependentPositionId bigint not null,
    PrincipalPositionId bigint not null,
    [Priority] int not null,
    ObjectBindingType int not null
)
create index IX_RulesetRule_Id ON PriceFacts.RulesetRule (Id)
go