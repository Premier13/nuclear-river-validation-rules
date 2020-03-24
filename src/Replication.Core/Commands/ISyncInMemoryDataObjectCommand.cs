using System;

namespace NuClear.Replication.Core.Commands
{
    public interface ISyncInMemoryDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}