using FluentMigrator;

namespace NuClear.ValidationRules.Migrator.Migrations
{
    [Migration(202003171500)]
    public class CopyPersistentFactsData : Migration
    {
        public override void Up()
        {
            Execute.EmbeddedScript("202003171500_CopyPersistentFactsData.sql");
        }

        public override void Down()
        {
        }
    }
}
