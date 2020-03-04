using System.Collections;
using System.Collections.Generic;
using LinqToDB.Data;

namespace NuClear.ValidationRules.Import.Processing.Writers
{
    public interface IEntityWriter
    {
        IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, ICollection data);
    }
}