using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public interface IRelationProvider<TValue> where TValue : class
    {
        IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<TValue> entities);
    }
}
