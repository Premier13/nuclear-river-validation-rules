using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace NuClear.ValidationRules.Migrator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var (connectionString, version) = ParseArgs(args);
            var serviceProvider = CreateServices(connectionString);

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                if(version.HasValue)
                    UpdateDatabase(scope.ServiceProvider, version.Value);
                else
                    UpdateDatabase(scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(connectionString)
                    .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
                    .ScanIn(typeof(Program).Assembly).For.Migrations()
                    .ScanIn(typeof(Program).Assembly).For.EmbeddedResources())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider, long targetVersion)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(targetVersion);
        }

        private static (string connectionString, long? version) ParseArgs(string[] args)
            => args.Length switch
            {
                1 => (args[0], null),
                2 => (args[0], long.Parse(args[1])),
                _ => throw new ArgumentException(
                    "Unsupported arguments. Expected connection string and optional version.")
            };
    }
}
