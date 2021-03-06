﻿using NuClear.Assembling.TypeProcessing;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NuClear.Messaging.API.Flows;
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

            if (args.Contains("-facts"))
            {
                if (tenants.Count != 0)
                {
                    commands.AddRange(BulkReplicationCommands.ErmToFacts
                        .Where(x => IsTenantConfigured(x, tenants)));
                    commands.Add(SchemaInitializationCommands.ErmFacts);
                }
                
                var flows = ParseFlows(args);
                if (flows.Count != 0)
                {
                    commands.AddRange(BulkReplicationCommands.KafkaToFacts
                        .Where(x => flows.Contains(x.MessageFlow)));
                    commands.AddRange(SchemaInitializationCommands.KafkaFacts
                        .Where(x => flows.Contains(x.Key))
                        .Select(x => x.Value));
                }
            }

            if (args.Contains("-aggregates"))
            {
                commands.Add(BulkReplicationCommands.FactsToAggregates);
                commands.Add(SchemaInitializationCommands.Aggregates);
            }

            if (args.Contains("-messages"))
            {
                commands.AddRange(BulkReplicationCommands.ErmToMessages
                    .Where(x => IsTenantConfigured(x, tenants)));
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
                new KafkaFactsBulkCommandFactory(kafkaSettingsFactory),
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

        private static IReadOnlyCollection<IMessageFlow> ParseFlows(IEnumerable<string> args) =>
            args.Where(x => x.StartsWith("-flows="))
                .SelectMany(x => x.Replace("-flows=", "").Split(','))
                .Select(x => (IMessageFlow)(x.ToUpperInvariant() switch
                {
                    "AMSFACTSFLOW" => AmsFactsFlow.Instance,
                    "RULESETFACTSFLOW" => RulesetFactsFlow.Instance,
                    "INFORUSSIAFACTSFLOW" => InfoRussiaFactsFlow.Instance,
                    "FIJIFACTSFLOW" => FijiFactsFlow.Instance,
                    _ => throw new NotSupportedException($"Unsupported flow {x}")
                })).ToHashSet();

        private static IReadOnlyCollection<Tenant> ParseTenants(IEnumerable<string> args) =>
            args.Where(x => x.StartsWith("-tenants="))
                .SelectMany(x => x.Replace("-tenants=", "").Split(','))
                .Select(x => Enum.Parse(typeof(Tenant), x))
                .Cast<Tenant>()
                .ToHashSet();

        private static bool IsTenantConfigured(ReplicateInBulkCommand command, IEnumerable<Tenant> tenants)
            => command.SourceStorageDescriptor.Tenant.HasValue &&
                tenants.Contains(command.SourceStorageDescriptor.Tenant.Value);
    }
}
