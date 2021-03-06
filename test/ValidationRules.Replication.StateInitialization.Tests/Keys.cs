﻿using System;

using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Hosting.Common.Identities.Connections;
using NuClear.ValidationRules.StateInitialization.Host;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        ReplicateInBulkCommand Command { get; }
    }

    public sealed class Facts : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(
                DataObjectTypesProvider.ErmFactTypes,
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                                       new StorageDescriptor(ValidationRulesConnectionStringIdentity.Instance, Schema.Facts),
                                       DbManagementMode.None);
    }

    public sealed class Aggregates : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(
                DataObjectTypesProvider.AggregateTypes,
                new StorageDescriptor(ValidationRulesConnectionStringIdentity.Instance, Schema.Facts),
                                       new StorageDescriptor(ValidationRulesConnectionStringIdentity.Instance, Schema.Aggregates),
                                       DbManagementMode.None);
    }

    public sealed class Messages : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(
                DataObjectTypesProvider.MessagesTypes,
                new StorageDescriptor(ValidationRulesConnectionStringIdentity.Instance, Schema.Aggregates),
                                       new StorageDescriptor(ValidationRulesConnectionStringIdentity.Instance, Schema.Messages),
                                       DbManagementMode.None);
    }
}
