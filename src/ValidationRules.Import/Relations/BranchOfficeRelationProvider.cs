using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class BranchOfficeRelationProvider : IRelationProvider<BranchOffice>
    {
        private const string BranchOfficeName = "NuClear.ValidationRules.Storage.Model.Facts.BranchOffice";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<BranchOffice> entities)
        {
            var orderRelations =
                from branchOffice in entities
                from boou in dataConnection.GetTable<BranchOfficeOrganizationUnit>()
                    .Where(x => x.BranchOfficeId == branchOffice.Id)
                from order in dataConnection.GetTable<OrderConsistency>()
                    .Where(x => x.BranchOfficeOrganizationUnitId == boou.Id)
                select new RelationRecord(BranchOfficeName, OrderName, order.Id);

            return orderRelations;
        }
    }
}
