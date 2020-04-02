using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AccountAccessor : IStorageBasedDataObjectAccessor<Account>, IDataChangesHandler<Account>
    {
        private readonly IQuery _query;

        public AccountAccessor(IQuery query) => _query = query;

        public IQueryable<Account> GetSource()
            => _query.For(Specs.Find.Erm.Account)
                .Select(x => new Account
                {
                    Id = x.Id,
                    Balance = x.Balance,
                });

        public FindSpecification<Account> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<Account>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Account> dataObjects)
            => new[] {new DataObjectCreatedEvent(typeof(Account), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Account> dataObjects)
            => new[] {new DataObjectUpdatedEvent(typeof(Account), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Account> dataObjects)
            => new[] {new DataObjectDeletedEvent(typeof(Account), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Account> dataObjects)
        {
            var accountIds = dataObjects.Select(x => x.Id).ToHashSet();

            var orderIds =
                (from bargain in _query.For<Bargain>().Where(x => accountIds.Contains(x.AccountId.Value))
                from order in _query.For<OrderConsistency>().Where(x => x.BargainId == bargain.Id)
                select order.Id)
                .Distinct()
                .ToList();

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(Account), typeof(Order), orderIds)};
        }
    }
}