using LinqToDB;

namespace NuClear.ValidationRules.Migrator
{
    internal interface IMigration
    {
        string Id { get; }
        string Name { get; }

        void Up(DataContext context);
    }
}