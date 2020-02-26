using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class CostPerClickCategoryRestrictionRelationProvider : IRelationProvider<CostPerClickCategoryRestriction>
    {
        private const string CostPerClickCategoryRestrictionName = "NuClear.ValidationRules.Storage.Model.Facts.CostPerClickCategoryRestriction";
        private const string ProjectName = "NuClear.ValidationRules.Storage.Model.Facts.Project";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<CostPerClickCategoryRestriction> entities)
        {
            var projectRelations =
                from restriction in entities
                select new RelationRecord(CostPerClickCategoryRestrictionName, ProjectName, restriction.ProjectId);

            return projectRelations;
        }
    }
}