using Confluent.Kafka;
using NuClear.Assembling.TypeProcessing;
using NuClear.Replication.Core;
using NuClear.River.Hosting.Common.Settings;
using NuClear.StateInitialization.Core.Actors;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.Hosting.Common;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.StateInitialization.Host.Assembling;
using NuClear.ValidationRules.StateInitialization.Host.Kafka;
using NuClear.ValidationRules.StateInitialization.Host.Kafka.Ams;
using NuClear.ValidationRules.StateInitialization.Host.Kafka.Rulesets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NuClear.Replication.Core.Tenancy;
using NuClear.StateInitialization.Core.Commands;
using NuClear.ValidationRules.Hosting.Common.Settings;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class Program
    {
        private static readonly TenantConnectionStringSettings ConnectionStringSettings =
            new TenantConnectionStringSettings();

        public static void Main(string[] args)
        {
            StateInitializationRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true,
                typeof(object));

            var commands = new List<ICommand>();
            var tenants = ParseTenants(args);

            if (args.Any(x => x.Contains("-facts")))
            {
                commands.AddRange(BulkReplicationCommands.ErmToFacts
                    .Where(x => IsConfigured(x, tenants)));
                commands.Add(new KafkaReplicationCommand(KafkaFactsFlow.Instance, BulkReplicationCommands.KafkaToFacts));
                // TODO: отдельный schema init для erm\ams\ruleset facts
                // мешает таблица EntityName, она общая и у Kafka и у Erm
                // отдельный schameinit будет перезатирать эту таблицу от другого schemainit, а должен мёрджить
                commands.Add(SchemaInitializationCommands.Facts);
            }

            if (args.Contains("-aggregates"))
            {
                commands.Add(BulkReplicationCommands.FactsToAggregates);
                commands.Add(SchemaInitializationCommands.Aggregates);
            }

            if (args.Contains("-messages"))
            {
                commands.AddRange(BulkReplicationCommands.ErmToMessages
                    .Where(x => IsConfigured(x, tenants)));
                commands.Add(BulkReplicationCommands.AggregatesToMessages);
                commands.Add(SchemaInitializationCommands.Messages);
            }

            if (args.Contains("-events"))
            {
                commands.Add(SchemaInitializationCommands.Events);
            }

            var environmentSettings = new EnvironmentSettingsAspect();
            var tracer = CreateTracer(environmentSettings);

            var kafkaSettingsFactory = new StateInitKafkaSettingsFactory(new KafkaSettingsFactory(ConnectionStringSettings));
            var kafkaMessageFlowReceiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), kafkaSettingsFactory);

            var bulkReplicationActor = new BulkReplicationActor(ConnectionStringSettings);
            var kafkaReplicationActor = new KafkaReplicationActor(ConnectionStringSettings,
                kafkaMessageFlowReceiverFactory,
                new KafkaMessageFlowInfoProvider(kafkaSettingsFactory),
                new IBulkCommandFactory<ConsumeResult<Ignore, byte[]>>[]
                {
                    new AmsFactsBulkCommandFactory(kafkaSettingsFactory),
                    new RulesetFactsBulkCommandFactory(kafkaSettingsFactory)
                },
                tracer);

            var schemaInitializationActor = new SchemaInitializationActor(ConnectionStringSettings);

            var sw = Stopwatch.StartNew();
            schemaInitializationActor.ExecuteCommands(commands);
            bulkReplicationActor.ExecuteCommands(commands.Where(x => BulkReplicationCommands.ErmToFacts.Contains(x))
                .ToList());
            kafkaReplicationActor.ExecuteCommands(commands);
            bulkReplicationActor.ExecuteCommands(commands.Where(x => !BulkReplicationCommands.ErmToFacts.Contains(x))
                .ToList());

            var webAppSchemaHelper = new WebAppSchemaInitializationHelper(ConnectionStringSettings);
            if (args.Contains("-webapp"))
            {
                webAppSchemaHelper.CreateWebAppSchema(SchemaInitializationCommands.WebApp);
            }

            if (args.Contains("-webapp-drop"))
            {
                webAppSchemaHelper.DropWebAppSchema(SchemaInitializationCommands.WebApp);
            }

            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }

        private static ITracer CreateTracer(IEnvironmentSettings environmentSettings)
        {
            return Log4NetTracerBuilder.Use
                .ApplicationXmlConfig
                .Console
                .WithGlobalProperties(x =>
                    x.Property("Environment", environmentSettings.EnvironmentName)
                        .Property(TracerContextKeys.EntryPoint, environmentSettings.EntryPointName)
                        .Property(TracerContextKeys.EntryPointHost, NetworkInfo.ComputerFQDN)
                        .Property(TracerContextKeys.EntryPointInstanceId, Guid.NewGuid().ToString()))
                .Build;
        }

        private static IReadOnlyCollection<Tenant> ParseTenants(IEnumerable<string> args)
        {
            var tenants = args.Where(x => x.StartsWith("-tenants="))
                .SelectMany(x => x.Replace("-tenants=", "").Split(','))
                .Select(x => Enum.Parse(typeof(Tenant), x))
                .Cast<Tenant>()
                .ToList();

            if (tenants.Count == 0)
                tenants.AddRange(Enum.GetValues(typeof(Tenant)).Cast<Tenant>());

            return new HashSet<Tenant>(tenants);
        }

        private static bool IsConfigured(ReplicateInBulkCommand command, IEnumerable<Tenant> tenants)
            => command.SourceStorageDescriptor.Tenant.HasValue &&
                tenants.Contains(command.SourceStorageDescriptor.Tenant.Value);
    }
}
