using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

namespace NuClear.ValidationRules.Migrator
{
    internal sealed class DatabaseAdapter
    {
        private readonly DataContext _dataContext;
        private readonly IReadOnlyCollection<MigrationInfo> _applied;

        public DatabaseAdapter(string connectionString)
        {
            _dataContext = new DataContext(SqlServerTools.GetDataProvider(SqlServerVersion.v2012), connectionString);
            _applied = GetAppliedMigrations(_dataContext).ToList();
        }

        public void Apply(IReadOnlyCollection<IMigration> available, string target)
        {
            var sequence = BuildSequence(available.ToLookup(x => x.Id), _applied.ToLookup(x => x.Id), target).ToList();
            foreach (var migration in sequence)
            {
                Console.WriteLine($"Applying migration {migration.Id}: {migration.Name}.");
                using var transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted});
                migration.Up(_dataContext);
                _dataContext.Insert(new MigrationInfo
                    {Id = migration.Id, Name = migration.Name, AppliedOn = DateTime.UtcNow});
                transaction.Complete();
            }
        }

        private static ITable<MigrationInfo> GetAppliedMigrations(DataContext dataContext)
        {
            try
            {
                return dataContext.GetTable<MigrationInfo>();
            }
            catch (Exception e) // todo: уточнить тип
            {
                Console.WriteLine(e);
                dataContext.CreateTable<MigrationInfo>();
                return dataContext.GetTable<MigrationInfo>();
            }
        }

        private static IReadOnlyCollection<IMigration> BuildSequence(
            ILookup<string, IMigration> available,
            ILookup<string, MigrationInfo> applied,
            string target)
        {
            var sequence = BuildSequence(available, applied).ToList();
            if (string.IsNullOrEmpty(target))
                return sequence;

            var index = sequence.FindIndex(x => string.Equals(x.Id, target, StringComparison.InvariantCultureIgnoreCase));
            return sequence.Take(index + 1).ToList();
        }

        private static IEnumerable<IMigration> BuildSequence(
            ILookup<string, IMigration> available,
            ILookup<string, MigrationInfo> applied)
        {
            var allKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            allKeys.UnionWith(available.Select(x => x.Key));
            allKeys.UnionWith(applied.Select(x => x.Key));

            var join = allKeys.Select(x => new
                {Id = x, Available = available[x].SingleOrDefault(), Applied = applied[x].SingleOrDefault()});

            // Конечный автомат для валидации последовательности применённых и будущих миграций.
            var allowedStates = new Dictionary<State, State[]>
            {
                {State.RemovedFromCode, new[] {State.Initial, State.RemovedFromCode}},
                {State.Applied, new[] {State.Initial, State.RemovedFromCode, State.Applied}},
                {State.NotApplied, new[] {State.Initial, State.RemovedFromCode, State.Applied, State.NotApplied}},
            };

            var state = State.Initial;
            foreach (var x in join)
            {
                // Первыми в последовательности могут идти миграции, которые уже удалили из кода, но не из БД.
                // Они должны идти непрерывным блоком в начале.
                if (x.Applied != null && x.Available == null && allowedStates[State.RemovedFromCode].Contains(state))
                {
                    state = State.RemovedFromCode;
                    continue;
                }

                // Далее могут идти известные мигратору миграции, которые были применены.
                // Аналогично, непрерывным блоком.
                if (x.Applied != null && x.Available != null && allowedStates[State.Applied].Contains(state))
                {
                    state = State.Applied;
                    continue;
                }

                // Наконец, миграции которые нужно применить. Непрерывным блоком.
                if (x.Applied == null && x.Available != null && allowedStates[State.NotApplied].Contains(state))
                {
                    state = State.NotApplied;
                    yield return x.Available;
                }

                // При обнаруженном нарушении последовательности,
                // например, неизвестная среди известных или неприменённая среди применнённых миграций,
                // обработку прекращаем, выводим ошибку.
                throw new Exception($"Invalid migrations state. Applied = {x.Applied?.Id ?? "null"}, " +
                    $"Available = {x.Available?.Id ?? "null"}," +
                    $"State = {state}");
            }
        }

        private enum State
        {
            Initial,
            RemovedFromCode,
            Applied,
            NotApplied,
        }

        [Table(Schema = "dbo")]
        public sealed class MigrationInfo
        {
            [PrimaryKey, MaxLength(32)] public string Id { get; set; }
            [MaxLength(64)] public string Name { get; set; }
            public DateTime AppliedOn { get; set; }
        }
    }
}
