﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Aggregates.PriceRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.OrderPeriod> orderPeriodBulkRepository,
            IBulkRepository<Order.OrderPricePosition> orderPricePositionBulkRepository,
            IBulkRepository<Order.OrderCategoryPosition> orderCategoryPositionBulkRepository,
            IBulkRepository<Order.OrderThemePosition> orderThemePositionBulkRepository,
            IBulkRepository<Order.AmountControlledPosition> amountControlledPositionBulkRepository,
            IBulkRepository<Order.ActualPrice> actualPriceBulkRepository,
            IBulkRepository<Order.EntranceControlledPosition> entranceControlledPositionBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new OrderPeriodAccessor(query), orderPeriodBulkRepository),
                HasValueObject(new OrderPricePositionAccessor(query), orderPricePositionBulkRepository),
                HasValueObject(new OrderCategoryPositionAccessor(query), orderCategoryPositionBulkRepository),
                HasValueObject(new OrderThemePositionAccessor(query), orderThemePositionBulkRepository),
                HasValueObject(new AmountControlledPositionAccessor(query), amountControlledPositionBulkRepository),
                HasValueObject(new ActualPriceAccessor(query), actualPriceBulkRepository),
                HasValueObject(new EntranceControlledPositionAccessor(query), entranceControlledPositionBulkRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                        MessageTypeCode.OrderMustHaveActualPrice,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   select new Order
                       {
                           Id = order.Id,
                           BeginDistribution = order.BeginDistribution,
                           EndDistributionPlan = order.EndDistributionPlan,
                           IsCommitted = Facts::Order.State.Committed.Contains(order.WorkflowStep)
                       };

            public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .ToHashSet();
                return new FindSpecification<Order>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class OrderPeriodAccessor : DataChangesHandler<Order.OrderPeriod>, IStorageBasedDataObjectAccessor<Order.OrderPeriod>
        {
            private readonly IQuery _query;

            public OrderPeriodAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                        MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                        MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictions,
                        MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictionsMass,
                        MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions
                    };

            public IQueryable<Order.OrderPeriod> GetSource()
                => GetSource1().Concat(GetSource2());

            public IQueryable<Order.OrderPeriod> GetSource1()
                => _query.For<Facts::Order>()
                        .Select(x => new Order.OrderPeriod
                        {
                            OrderId = x.Id,
                            Begin = x.BeginDistribution,
                            End = x.EndDistributionFact,
                            Scope = Scope.Compute(x.WorkflowStep, x.Id)
                        });

            public IQueryable<Order.OrderPeriod> GetSource2()
                => _query.For<Facts::Order>()
                        .Where(x => x.EndDistributionFact != x.EndDistributionPlan)
                        .Select(x => new Order.OrderPeriod
                        {
                            OrderId = x.Id,
                            Begin = x.EndDistributionFact,
                            End = x.EndDistributionPlan,
                            Scope = x.Id
                        });

            public FindSpecification<Order.OrderPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.OrderPeriod>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderPricePositionAccessor : DataChangesHandler<Order.OrderPricePosition>, IStorageBasedDataObjectAccessor<Order.OrderPricePosition>
        {
            private readonly IQuery _query;

            public OrderPricePositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                    };

            public IQueryable<Order.OrderPricePosition> GetSource()
                =>
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    select new Order.OrderPricePosition
                       {
                           OrderId = orderPosition.OrderId,
                           OrderPositionId = orderPosition.Id,
                           PositionId = pricePosition.PositionId,

                           PriceId = pricePosition.PriceId,
                           IsActive = pricePosition.IsActiveNotDeleted,
                       };


            public FindSpecification<Order.OrderPricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.OrderPricePosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderCategoryPositionAccessor : DataChangesHandler<Order.OrderCategoryPosition>, IStorageBasedDataObjectAccessor<Order.OrderCategoryPosition>
        {
            private readonly IQuery _query;

            public OrderCategoryPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                    };

            public IQueryable<Order.OrderCategoryPosition> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue) on orderPosition.Id equals opa.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => x.CategoryCode == Facts::Position.CategoryCodeAdvertisementInCategory) on opa.PositionId equals position.Id // join для того, чтобы отбросить неподходящие продажи
                    join project in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals project.OrganizationUnitId
                    select new Order.OrderCategoryPosition
                    {
                        OrderId = order.Id,
                        ProjectId = project.Id,
                        OrderPositionAdvertisementId = opa.Id,
                        CategoryId = opa.CategoryId.Value,
                    };

                return result;
            }

            public FindSpecification<Order.OrderCategoryPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.OrderCategoryPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderThemePositionAccessor : DataChangesHandler<Order.OrderThemePosition>, IStorageBasedDataObjectAccessor<Order.OrderThemePosition>
        {
            private readonly IQuery _query;

            public OrderThemePositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                    };

            public IQueryable<Order.OrderThemePosition> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.ThemeId.HasValue) on orderPosition.Id equals opa.OrderPositionId
                    join project in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals project.OrganizationUnitId
                    select new Order.OrderThemePosition
                    {
                            OrderId = order.Id,
                            ProjectId = project.Id,
                            OrderPositionAdvertisementId = opa.Id,
                            ThemeId = opa.ThemeId.Value,
                        };

                return result;
            }

            public FindSpecification<Order.OrderThemePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.OrderThemePosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AmountControlledPositionAccessor : DataChangesHandler<Order.AmountControlledPosition>, IStorageBasedDataObjectAccessor<Order.AmountControlledPosition>
        {
            private readonly IQuery _query;

            public AmountControlledPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                        MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictions,
                        MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictionsMass,
                    };

            public IQueryable<Order.AmountControlledPosition> GetSource()
                =>  from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join adv in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals adv.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted && x.IsControlledByAmount) on adv.PositionId equals position.Id
                    join project in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals project.OrganizationUnitId
                    select new Order.AmountControlledPosition
                    {
                        OrderId = orderPosition.OrderId,
                        OrderPositionId = orderPosition.Id,
                        CategoryCode = position.CategoryCode,
                        ProjectId = project.Id,
                    };

            public FindSpecification<Order.AmountControlledPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.AmountControlledPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class EntranceControlledPositionAccessor : DataChangesHandler<Order.EntranceControlledPosition>, IStorageBasedDataObjectAccessor<Order.EntranceControlledPosition>
        {
            private readonly IQuery _query;

            public EntranceControlledPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions
                    };

            public IQueryable<Order.EntranceControlledPosition> GetSource()
                => (from order in _query.For<Facts::Order>()
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join adv in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals adv.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => Facts.Position.CategoryCodesPoiAddressCheck.Contains(x.CategoryCode)) on adv.PositionId equals position.Id
                    join address in _query.For<Facts::FirmAddress>().Where(x => x.EntranceCode != null) on adv.FirmAddressId equals address.Id
                    select new Order.EntranceControlledPosition
                    {
                        OrderId = orderPosition.OrderId,
                        EntranceCode = address.EntranceCode.Value,
                        FirmAddressId = address.Id,
                    }).Distinct();

            public FindSpecification<Order.EntranceControlledPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.EntranceControlledPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class ActualPriceAccessor : DataChangesHandler<Order.ActualPrice>, IStorageBasedDataObjectAccessor<Order.ActualPrice>
        {
            private readonly IQuery _query;

            public ActualPriceAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                        MessageTypeCode.OrderMustHaveActualPrice,
                    };

            public IQueryable<Order.ActualPrice> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    join destProject in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals destProject.OrganizationUnitId
                    let price = _query.For<Facts::Price>()
                                .Where(x => x.ProjectId == destProject.Id)
                                .Where(x => x.BeginDate <= order.BeginDistribution)
                                .OrderByDescending(x => x.BeginDate)
                                .FirstOrDefault()
                    select new Order.ActualPrice
                    {
                        OrderId = order.Id,
                        PriceId = price != null ? (long?)price.Id : null
                    };

                return result;
            }

            public FindSpecification<Order.ActualPrice> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).ToHashSet();
                return new FindSpecification<Order.ActualPrice>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}