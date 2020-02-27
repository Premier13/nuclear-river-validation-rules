using System;
using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class NomenclatureCategoryRelationProvider : IRelationProvider<NomenclatureCategory>
    {
        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<NomenclatureCategory> entities)
        {
            // тип ни на что не влияет. он вообще в таком случае нужен?
            // ps. сам - может и не нужен, но его название - да.
            return Array.Empty<RelationRecord>().AsQueryable();
        }
    }
}