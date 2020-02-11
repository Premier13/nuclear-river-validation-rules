using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Jobs.RemoteControl.PortResolver;
using Jobs.RemoteControl.Provider;
using Jobs.RemoteControl.Registrar;
using Jobs.RemoteControl.Settings;

using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.Assembling.TypeProcessing;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Transports;
using NuClear.ValidationRules.Replication.Host.Factories;
using NuClear.ValidationRules.Replication.Host.Factories.Messaging.Processor;
using NuClear.ValidationRules.Replication.Host.Factories.Messaging.Receiver;
using NuClear.ValidationRules.Replication.Host.Factories.Messaging.Transformer;
using NuClear.ValidationRules.Replication.Host.Factories.Replication;
using NuClear.ValidationRules.Replication.Host.Settings;
using NuClear.ValidationRules.Storage;
using NuClear.DI.Unity.Config;
using NuClear.DI.Unity.Config.RegistrationResolvers;
using NuClear.Jobs;
using NuClear.Jobs.Schedulers;
using NuClear.Jobs.Schedulers.Exporter;
using NuClear.Jobs.Settings;
using NuClear.Jobs.Unity;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Actors.Validators;
using NuClear.Messaging.API.Processing.Audit;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Accumulators;
using NuClear.Messaging.DI.Factories.Unity.Common;
using NuClear.Messaging.DI.Factories.Unity.Handlers;
using NuClear.Messaging.DI.Factories.Unity.Processors;
using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Stages;
using NuClear.Messaging.DI.Factories.Unity.Transformers;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Validators;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Messaging.Transports.ServiceBus.LockRenewer;
using NuClear.Metamodeling.Domain.Processors.Concrete;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Metamodeling.Validators;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.API;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Replication.Core.Settings;
using NuClear.Replication.OperationsProcessing.Metadata;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Security;
using NuClear.Security.API.Auth;
using NuClear.Security.API.Context;
using NuClear.Settings.Unity;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;
using NuClear.Storage.LinqToDB.Writings;
using NuClear.Storage.Readings;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.Hosting.Common;
using NuClear.ValidationRules.Hosting.Common.Identities.Connections;
using NuClear.ValidationRules.Hosting.Common.Settings.Kafka;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.RulesetFactsFlow;
using NuClear.ValidationRules.Storage.Model.Facts;
using NuClear.ValidationRules.Replication.Accessors;
using NuClear.ValidationRules.Replication.Accessors.Rulesets;
using NuClear.ValidationRules.Replication.Host.Jobs;
using NuClear.ValidationRules.Storage.FieldComparer;

using Quartz.Spi;
using Schema = NuClear.ValidationRules.Storage.Schema;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureTracing(this IUnityContainer container)
        {
            return container.RegisterType<ITracer>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(CreateTracer));
        }

        private static ITracer CreateTracer(IUnityContainer container)
        {
            var environmentSettings = container.Resolve<IEnvironmentSettings>();
            var connectionStringSettings = container.Resolve<IConnectionStringSettings>();
            var logstashConnectionString = new Uri(
                connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance));

            return Log4NetTracerBuilder.Use
                .ApplicationXmlConfig
                .Console
                .EventLog
                .WithGlobalProperties(x =>
                    x.Property("Environment", environmentSettings.EnvironmentName)
                        .Property(TracerContextKeys.EntryPoint, environmentSettings.EntryPointName)
                        .Property(TracerContextKeys.EntryPointHost, NetworkInfo.ComputerFQDN)
                        .Property(TracerContextKeys.EntryPointInstanceId, Guid.NewGuid().ToString()))
                .Logstash(logstashConnectionString, appender => { appender.LogstashLayout.IncrementalCounter = true; })
                .Build;
        }
    }
}
