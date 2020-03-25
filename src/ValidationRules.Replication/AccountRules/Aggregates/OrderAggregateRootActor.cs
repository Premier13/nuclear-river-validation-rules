using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Aggregates.AccountRules;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> orderBulkRepository,
            IBulkRepository<Order.DebtPermission> debtPermissionBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), orderBulkRepository,
                HasValueObject(new DebtPermissionAccessor(query), debtPermissionBulkRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query) : base(CreateInvalidator()) => _query = query;

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        {MessageTypeCode.AccountBalanceShouldBePositive, GetRelatedOrders},
                        {MessageTypeCode.AccountShouldExist, GetRelatedOrders},
                    };

            private static IEnumerable<long> GetRelatedOrders(IReadOnlyCollection<Order> dataObjects)
                => dataObjects.Select(x => x.Id);
            
            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderConsistency in _query.For<Facts::OrderConsistency>().Where(x => x.Id == order.Id)
                   from orderWorkflow in _query.For<Facts::OrderWorkflow>().Where(x => Facts::OrderWorkflowStep.Payable.Contains(x.Step)).Where(x => x.Id == order.Id)
                   from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == orderConsistency.LegalPersonId && x.BranchOfficeOrganizationUnitId == orderConsistency.BranchOfficeOrganizationUnitId).DefaultIfEmpty()

                // Временный костыль чтобы отфильтровать все существующие на данный момент rocket-data заказы
                // вместо этого костыля надо добавить полноценную поддержку rocket-data заказов в VR до 1 мая 2020
                where !new []
                {
                    993400733305078534,
                    1031470311579903799,
                    1033782690185908706,
                    1044879683426403160,
                    1018783740271017030,
                    1058076891813980160,
                    1055961806661359560,
                    1058051681664120833,
                    1046319024075619240,
                    1049783413232780497,
                    1073767193633763331,
                    1072043765006759171,
                    1083488388720991232,
                    1084293870000744195,
                    1084284440686018048,
                    1084284517072275971,
                    1084299911002363907,
                    1084293940591230979,
                    1084299910999449091,
                    1084299910999469827,
                    1083696271635024640,
                    1083696271637489408,
                    1073767267428367360,
                    1084299836504324608,
                    1084299836504397568,
                    1084299911000187651,
                    1084299911000218627,
                }.Contains(order.Id)
                select new Order
                {
                    Id = order.Id,
                    AccountId = account.Id,
                    IsFreeOfCharge = orderConsistency.IsFreeOfCharge,
                    Start = order.AgileDistributionStartDate,
                    End = order.AgileDistributionEndFactDate,
                };

            public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
                return new FindSpecification<Order>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class DebtPermissionAccessor : DataChangesHandler<Order.DebtPermission>, IStorageBasedDataObjectAccessor<Order.DebtPermission>
        {
            private readonly IQuery _query;

            public DebtPermissionAccessor(IQuery query) : base(CreateInvalidator()) => _query = query;

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        {MessageTypeCode.AccountBalanceShouldBePositive, GetRelatedOrders}
                    };

            private static IEnumerable<long> GetRelatedOrders(IReadOnlyCollection<Order.DebtPermission> dataObjects)
                => dataObjects.Select(x => x.OrderId);

            public IQueryable<Order.DebtPermission> GetSource()
                => from x in _query.For<Facts::UnlimitedOrder>()
                   select new Order.DebtPermission
                       {
                           OrderId = x.OrderId,
                           Start = x.PeriodStart,
                           End = x.PeriodEnd,
                       };

            public FindSpecification<Order.DebtPermission> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().SelectMany(c => c.AggregateRootIds).ToHashSet();
                return new FindSpecification<Order.DebtPermission>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}