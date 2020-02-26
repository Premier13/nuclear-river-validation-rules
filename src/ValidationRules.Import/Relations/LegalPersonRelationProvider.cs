using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class LegalPersonRelationProvider : IRelationProvider<LegalPerson>
    {
        private const string LegalPersonName = "NuClear.ValidationRules.Storage.Model.Facts.LegalPerson";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<LegalPerson> entities)
        {
            var orderRelations =
                from legalPerson in entities
                from order in dataConnection.GetTable<OrderConsistency>()
                    .Where(x => x.LegalPersonId == legalPerson.Id)
                select new RelationRecord(LegalPersonName, OrderName, order.Id);

            return orderRelations;
        }
    }
}