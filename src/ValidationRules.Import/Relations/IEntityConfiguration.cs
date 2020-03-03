using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public interface IEntityConfiguration
    {
        void Apply(Cache cache);
        void Apply(FluentMappingBuilder builder);
        void Apply(Writer writer);
    }

    public interface IRelationProvider<T>
    {
        IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<T> actual, IQueryable<T> outdated);
    }
}
