﻿using System;
using System.Configuration;
using System.Diagnostics;

using NuClear.ValidationRules.Domain;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.Api;
using NuClear.Replication.Bulk.Api.Factories;
using NuClear.Replication.Bulk.Api.Storage;

namespace NuClear.ValidationRules.StateInitialization.EntryPoint
{
    public sealed class Program
    {
        private static readonly MetadataProvider DefaultProvider
            = new MetadataProvider(
                new IMetadataSource[]
                {
                    new BulkReplicationMetadataSource(),
                    new FactsReplicationMetadataSource(),
                    new ImportOrderValidationConfigMetadataSource(),
                },
                new IMetadataProcessor[] { new ReferencesEvaluatorProcessor() });

        public static void Main(string[] args)
        {
            var connectionStringSettings = new StateInitializationConnectionStringSettings(ConfigurationManager.ConnectionStrings);
            var viewRemover = new ViewRemover(connectionStringSettings);
            var connectionFactory = new DataConnectionFactory(connectionStringSettings);
            var runner = new BulkReplicationRunner(DefaultProvider, connectionFactory, viewRemover);

            foreach (var mode in args)
            {
                var sw = Stopwatch.StartNew();
                runner.Run(mode);
                Console.WriteLine($"{mode}, {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
