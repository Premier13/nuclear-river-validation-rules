using FluentMigrator;

namespace NuClear.ValidationRules.Migrator.Migrations
{
    [Migration(202003191700)]
    public class FixEntityNameTable : Migration
    {
        public override void Up()
        {
            Alter.Table("EntityName")
                .InSchema("PersistentFacts")
                .AlterColumn("Name").AsString(int.MaxValue);
        }

        public override void Down()
        {
        }
    }
}
