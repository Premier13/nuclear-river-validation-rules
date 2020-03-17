insert into [PersistentFacts].[Account]([Id], [BranchOfficeOrganizationUnitId], [LegalPersonId], [Balance])
select [Id], [BranchOfficeOrganizationUnitId], [LegalPersonId], [Balance]
from [Facts].[Account]

insert into [PersistentFacts].[AccountDetail]([Id], [IsDeleted], [AccountId], [OrderId], [PeriodStartDate])
select [Id], 0, [AccountId], [OrderId], [PeriodStartDate]
from [Facts].[AccountDetail]

insert into [PersistentFacts].[BranchOffice]([Id], [IsDeleted])
select [Id], 0
from [Facts].[BranchOffice]

insert into [PersistentFacts].[BranchOfficeOrganizationUnit]([Id], [BranchOfficeId], [IsDeleted])
select [Id], [BranchOfficeId], 0
from [Facts].[BranchOfficeOrganizationUnit]

insert into [PersistentFacts].[Category]([Id], [L1Id], [L2Id], [L3Id], [IsDeleted])
select [Id], [L1Id], [L2Id], [L3Id], 0
from [Facts].[Category]

insert into [PersistentFacts].[CategoryProject]([ProjectId], [CategoryId])
select [Project].[Id], [CategoryOrganizationUnit].[CategoryId]
from [Facts].[CategoryOrganizationUnit]
         join [Facts].[Project] on [Project].[OrganizationUnitId] = [CategoryOrganizationUnit].[OrganizationUnitId]

insert into [PersistentFacts].[CostPerClickCategoryRestriction]([ProjectId], [Start], [CategoryId], [MinCostPerClick])
select [ProjectId], [Start], [CategoryId], [MinCostPerClick]
from [Facts].[CostPerClickCategoryRestriction]

insert into [PersistentFacts].[LegalPerson]([Id], [IsDeleted])
select [Id], 0
from [Facts].[LegalPerson]

insert into [PersistentFacts].[LegalPersonProfile]([Id], [IsDeleted], [LegalPersonId], [BargainEndDate], [WarrantyEndDate])
select [Id], 0, [LegalPersonId], [BargainEndDate], [WarrantyEndDate]
from [Facts].[LegalPersonProfile]

insert into [PersistentFacts].[NomenclatureCategory]([Id], [IsDeleted])
select [Id], 0
from [Facts].[NomenclatureCategory]

insert into [PersistentFacts].[Position]([Id], [BindingObjectType], [SalesModel], [PositionsGroup], [IsCompositionOptional], [ContentSales], [IsControlledByAmount], [CategoryCode], [IsDeleted])
select [Id], [BindingObjectType], [SalesModel], [PositionsGroup], [IsCompositionOptional], [ContentSales], [IsControlledByAmount], [CategoryCode], [IsDeleted]
from [Facts].[Position]

insert into [PersistentFacts].[PositionChild]([MasterPositionId], [ChildPositionId])
select [MasterPositionId], [ChildPositionId]
from [Facts].[PositionChild]

insert into [PersistentFacts].[Price]([Id], [ProjectId], [BeginDate], [IsDeleted])
select [Id], [ProjectId], [BeginDate], 0
from [Facts].[Price]

insert into [PersistentFacts].[PricePosition]([Id], [PriceId], [PositionId], [IsActiveNotDeleted])
select [Id], [PriceId], [PositionId], [IsActiveNotDeleted]
from [Facts].[PricePosition]

insert into [PersistentFacts].[SalesModelCategoryRestriction]([ProjectId], [Start], [CategoryId], [SalesModel])
select [ProjectId], [Start], [CategoryId], [SalesModel]
from [Facts].[SalesModelCategoryRestriction]
