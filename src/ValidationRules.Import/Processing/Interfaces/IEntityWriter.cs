using System.Collections;
using System.Collections.Generic;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model;

namespace NuClear.ValidationRules.Import.Processing.Interfaces
{
    public interface IEntityWriter
    {
        IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, ICollection data);
    }
}
