using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class LegalPersonProfileRelationProvider : IRelationProvider<LegalPersonProfile>
    {
        private const string LegalPersonProfileName = "NuClear.ValidationRules.Storage.Model.Facts.LegalPersonProfile";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<LegalPersonProfile> entities)
        {
            var orderRelations =
                from legalPersonProfile in entities
                from order in dataConnection.GetTable<OrderConsistency>()
                    .Where(x => x.LegalPersonId == legalPersonProfile.LegalPersonId)
                select new RelationRecord(LegalPersonProfileName, OrderName, order.Id);

            return orderRelations;
        }
    }
}