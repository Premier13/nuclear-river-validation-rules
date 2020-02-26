using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class SalesModelCategoryRestrictionRelationProvider : IRelationProvider<SalesModelCategoryRestriction>
    {
        private const string SalesModelCategoryRestrictionName = "NuClear.ValidationRules.Storage.Model.Facts.SalesModelCategoryRestriction";
        private const string ProjectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<SalesModelCategoryRestriction> entities)
        {
            var projectRelations =
                from restriction in entities
                select new RelationRecord(SalesModelCategoryRestrictionName, ProjectName, restriction.ProjectId);

            return projectRelations;
        }
    }
}