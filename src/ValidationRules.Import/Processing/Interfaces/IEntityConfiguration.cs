using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model;

namespace NuClear.ValidationRules.Import.Processing.Interfaces
{
    public interface IEntityConfiguration
    {
        void Apply(FluentMappingBuilder builder);
        void Apply(CacheSaver cacheSaver, bool enableRelations);
    }

    public interface IRelationProvider<T>
    {
        IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<T> actual, IQueryable<T> outdated);
    }
}
