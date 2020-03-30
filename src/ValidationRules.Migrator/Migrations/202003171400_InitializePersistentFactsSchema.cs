using FluentMigrator;

namespace NuClear.ValidationRules.Migrator.Migrations
{
    [Migration(202003171400)]
    public class InitializePersistentFactsSchema : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Schema("PersistentFacts");

            Create.Table("Account")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BranchOfficeOrganizationUnitId").AsInt64()
                .WithColumn("LegalPersonId").AsInt64()
                .WithColumn("Balance").AsDecimal(19, 4);

            Create.Table("AccountDetail")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("IsDeleted").AsBoolean()
                .WithColumn("AccountId").AsInt64()
                .WithColumn("OrderId").AsInt64()
                .WithColumn("PeriodStartDate").AsDateTime2();

            Create.Table("BranchOffice")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("BranchOfficeOrganizationUnit")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BranchOfficeId").AsInt64()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("Category")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("L1Id").AsInt64().Nullable()
                .WithColumn("L2Id").AsInt64().Nullable()
                .WithColumn("L3Id").AsInt64().Nullable()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("CategoryProject")
                .InSchema("PersistentFacts")
                .WithColumn("ProjectId").AsInt64().PrimaryKey()
                .WithColumn("CategoryId").AsInt64().PrimaryKey();

            Create.Table("CostPerClickCategoryRestriction")
                .InSchema("PersistentFacts")
                .WithColumn("ProjectId").AsInt64().PrimaryKey()
                .WithColumn("Start").AsDateTime2().PrimaryKey()
                .WithColumn("CategoryId").AsInt64().PrimaryKey()
                .WithColumn("MinCostPerClick").AsDecimal();

            Create.Table("EntityName")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("EntityType").AsInt32().PrimaryKey()
                .WithColumn("TenantId").AsInt32().PrimaryKey()
                .WithColumn("Name").AsString();

            Create.Table("LegalPerson")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("LegalPersonProfile")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("IsDeleted").AsBoolean()
                .WithColumn("LegalPersonId").AsInt64()
                .WithColumn("BargainEndDate").AsDateTime2().Nullable()
                .WithColumn("WarrantyEndDate").AsDateTime2().Nullable();

            Create.Table("NomenclatureCategory")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey();

            Create.Table("Position")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BindingObjectType").AsInt32()
                .WithColumn("SalesModel").AsInt32()
                .WithColumn("PositionsGroup").AsInt32()
                .WithColumn("IsCompositionOptional").AsBoolean()
                .WithColumn("ContentSales").AsInt32()
                .WithColumn("IsControlledByAmount").AsBoolean()
                .WithColumn("CategoryCode").AsInt64()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("PositionChild")
                .InSchema("PersistentFacts")
                .WithColumn("MasterPositionId").AsInt64().PrimaryKey()
                .WithColumn("ChildPositionId").AsInt64().PrimaryKey();

            Create.Table("Price")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("ProjectId").AsInt64()
                .WithColumn("BeginDate").AsDateTime2()
                .WithColumn("IsDeleted").AsBoolean();

            Create.Table("PricePosition")
                .InSchema("PersistentFacts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("PriceId").AsInt64()
                .WithColumn("PositionId").AsInt64()
                .WithColumn("IsActiveNotDeleted").AsBoolean();

            Create.Table("SalesModelCategoryRestriction")
                .InSchema("PersistentFacts")
                .WithColumn("ProjectId").AsInt64().PrimaryKey()
                .WithColumn("Start").AsDateTime2().PrimaryKey()
                .WithColumn("CategoryId").AsInt64().PrimaryKey()
                .WithColumn("SalesModel").AsInt32();
        }
    }
}
