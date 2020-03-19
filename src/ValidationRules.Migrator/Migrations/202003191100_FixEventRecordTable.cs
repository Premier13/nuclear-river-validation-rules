using FluentMigrator;

namespace NuClear.ValidationRules.Migrator.Migrations
{
    [Migration(202003191100)]
    public class FixEventRecordTable : Migration
    {
        public override void Up()
        {
            Delete.Table("EventRecord")
                .InSchema("Service");

            Create.Table("EventRecord")
                .InSchema("Service")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Flow").AsGuid()
                .WithColumn("Content").AsString(int.MaxValue);
        }

        public override void Down()
        {
        }
    }
}
