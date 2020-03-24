using System;
using System.Collections.Generic;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class SyncInMemoryDataObjectCommand : ISyncInMemoryDataObjectCommand
    {
        public Type DataObjectType { get; }
        public IEnumerable<object> Dtos { get; }

        public SyncInMemoryDataObjectCommand(Type dataObjectType, IEnumerable<object> dtos) =>
            (DataObjectType, Dtos) = (dataObjectType, dtos);
    }
}