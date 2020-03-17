using System;
using System.Diagnostics;
using LinqToDB.Data;

namespace NuClear.ValidationRules.Migrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1);

            var (connectionString, version) = ParseArgs(args);
            var db = new DatabaseAdapter(connectionString);
            var migrations = new MigrationsAdapter(typeof(Program).Assembly);
            db.Apply(migrations.GetMigrations(), version);
        }

        private static (string, string) ParseArgs(string[] args)
            => args.Length switch
            {
                1 => (args[0], null),
                2 => (args[0], args[1]),
                _ => throw new ArgumentException(
                    "Unsupported arguments. Expected connection string and optional version.")
            };
    }
}
