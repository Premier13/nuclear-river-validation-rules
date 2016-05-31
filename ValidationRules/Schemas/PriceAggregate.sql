﻿if not exists (select * from sys.schemas where name = 'PriceAggregate') exec('create schema PriceAggregate')

if object_id('PriceAggregate.Position') is not null drop table PriceAggregate.Position

if object_id('PriceAggregate.OrderPeriod') is not null drop table PriceAggregate.OrderPeriod
if object_id('PriceAggregate.PricePeriod') is not null drop table PriceAggregate.PricePeriod
if object_id('PriceAggregate.Period') is not null drop table PriceAggregate.Period

if object_id('PriceAggregate.OrderPrice') is not null drop table PriceAggregate.OrderPrice
if object_id('PriceAggregate.OrderPricePosition') is not null drop table PriceAggregate.OrderPricePosition
if object_id('PriceAggregate.OrderPosition') is not null drop table PriceAggregate.OrderPosition
if object_id('PriceAggregate.[Order]') is not null drop table PriceAggregate.[Order]

if object_id('PriceAggregate.PriceDeniedPosition') is not null drop table PriceAggregate.PriceDeniedPosition
if object_id('PriceAggregate.PriceAssociatedPosition') is not null drop table PriceAggregate.PriceAssociatedPosition
if object_id('PriceAggregate.AdvertisementAmountRestriction') is not null drop table PriceAggregate.AdvertisementAmountRestriction
if object_id('PriceAggregate.Price') is not null drop table PriceAggregate.Price

if object_id('PriceAggregate.RulesetRule') is not null drop table PriceAggregate.RulesetRule
if object_id('PriceAggregate.Ruleset') is not null drop table PriceAggregate.Ruleset

go


-- price aggregate

create table PriceAggregate.Price(
    Id bigint NOT NULL,
    constraint PK_Price primary key (Id)
)
go

create table PriceAggregate.PriceDeniedPosition(
	PriceId bigint NOT NULL,
	DeniedPositionId bigint NOT NULL,
    PrincipalPositionId bigint NOT NULL,
    ObjectBindingType int NOT NULL
)
create index IX_PriceDeniedPosition_PriceId ON PriceAggregate.PriceDeniedPosition (PriceId)
go

create table PriceAggregate.PriceAssociatedPosition(
	PriceId bigint NOT NULL,
    AssociatedPositionId bigint NOT NULL,
    PrincipalPositionId bigint NOT NULL,
    ObjectBindingType int NOT NULL,
	GroupId bigint NOT NULL
)
create index IX_PriceAssociatedPosition_PriceId ON PriceAggregate.PriceAssociatedPosition (PriceId)
go

create table PriceAggregate.AdvertisementAmountRestriction(
    PriceId bigint NOT NULL,
    PositionId bigint NOT NULL,
    [Min] int NOT NULL,
    [Max] int NOT NULL,
    MissingMinimalRestriction bit NOT NULL,
)
create index IX_AdvertisementAmountRestriction_PriceId ON PriceAggregate.AdvertisementAmountRestriction (PriceId)
go

-- ruleset aggregate
create table PriceAggregate.Ruleset(
    Id bigint NOT NULL,
    constraint PK_Ruleset primary key (Id)
)
go

create table PriceAggregate.RulesetRule(
	RulesetId bigint NOT NULL,
	RuleType int NOT NULL,
	DependentPositionId bigint NOT NULL,
    PrincipalPositionId bigint NOT NULL,
    ObjectBindingType int NOT NULL
)
create index IX_RulesetRule_RulesetId ON PriceAggregate.RulesetRule (RulesetId)
go

-- order aggregate
create table PriceAggregate.[Order](
    Id bigint NOT NULL,
    FirmId bigint NOT NULL,
    constraint PK_Order primary key (Id)
)
go

create table PriceAggregate.OrderPosition(
    OrderId bigint NOT NULL,
    CompareMode int NOT NULL,
    PackagePositionId bigint NOT NULL,
    ItemPositionId bigint NOT NULL,
    Category3Id bigint NULL,
    Category1Id bigint NULL,
    FirmAddressId bigint NULL
)
create index IX_OrderPosition_OrderId ON PriceAggregate.OrderPosition (OrderId)
go

create table PriceAggregate.OrderPricePosition(
    OrderId bigint NOT NULL,
	OrderPositionId bigint NOT NULL,
	PriceId bigint NOT NULL
)
create index IX_OrderPricePosition_OrderId ON PriceAggregate.OrderPricePosition (OrderId)
go

-- period aggregate
create table PriceAggregate.Period(
    OrganizationUnitId bigint NOT NULL,
    [Start] datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL,
)
go

create table PriceAggregate.OrderPeriod(
    OrderId bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
)
go

create table PriceAggregate.PricePeriod(
    PriceId bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
)
go

-- position aggregate
create table PriceAggregate.Position(
    Id bigint NOT NULL,
    CategoryCode bigint NOT NULL,
    IsControlledByAmount bit NOT NULL,
    Name nvarchar(max) NOT NULL,
    constraint PK_Position primary key (Id)
)
go