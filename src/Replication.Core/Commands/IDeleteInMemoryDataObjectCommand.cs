using System;

namespace NuClear.Replication.Core.Commands
{
    public interface IDeleteInMemoryDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}