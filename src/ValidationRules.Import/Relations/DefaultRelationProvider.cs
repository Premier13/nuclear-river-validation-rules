using System;
using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class DefaultRelationProvider<T> : IRelationProvider<T> where T : class
    {
        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<T> entities)
            => Array.Empty<RelationRecord>().AsQueryable();
    }
}
