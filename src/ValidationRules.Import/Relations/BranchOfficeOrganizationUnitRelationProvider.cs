using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class BranchOfficeOrganizationUnitRelationProvider : IRelationProvider<BranchOfficeOrganizationUnit>
    {
        private const string BranchOfficeOrganizationUnitName = "NuClear.ValidationRules.Storage.Model.Facts.BranchOfficeOrganizationUnit";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<BranchOfficeOrganizationUnit> entities)
        {
            var orderRelations =
                from boou in entities
                from order in dataConnection.GetTable<OrderConsistency>()
                    .Where(x => x.BranchOfficeOrganizationUnitId == boou.Id)
                select new RelationRecord(BranchOfficeOrganizationUnitName, OrderName, order.Id);

            return orderRelations;
        }
    }
}
