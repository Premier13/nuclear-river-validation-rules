using System.Linq;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.Facts;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class AccountRelationProvider : IRelationProvider<Account>
    {
        // Строковые имена используются "большим" сервисом при десериализации.
        // todo: пересмотреть формат событий, текущие проблемы: 1 - избыточность по длине, 2 - названия типов.
        private const string AccountName = "NuClear.ValidationRules.Storage.Model.Facts.Account";
        private const string OrderName = "NuClear.ValidationRules.Storage.Model.Facts.Order";

        public IQueryable<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<Account> entities)
        {
            var orderRelations =
                from account in entities
                from order in dataConnection.GetTable<OrderConsistency>()
                    .Where(x => x.LegalPersonId == account.LegalPersonId && x.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId)
                select new RelationRecord(AccountName, OrderName, order.Id);

            var accountRelations =
                from account in entities
                select new RelationRecord(AccountName, AccountName, account.Id);

            return orderRelations.Union(accountRelations);
        }
    }
}
