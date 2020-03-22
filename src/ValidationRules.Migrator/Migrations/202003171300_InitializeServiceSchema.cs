using FluentMigrator;

namespace NuClear.ValidationRules.Migrator.Migrations
{
    [Migration(202003171300)]
    public class InitializeServiceSchema : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Schema("Service");

            Create.Table("ConsumerState")
                .InSchema("Service")
                .WithColumn("Topic").AsString(128).PrimaryKey()
                .WithColumn("Partition").AsInt32().PrimaryKey()
                .WithColumn("Offset").AsInt64();

            Create.Table("EventRecord")
                .InSchema("Service")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Flow").AsInt64()
                .WithColumn("Content").AsInt64();
        }
    }
}
