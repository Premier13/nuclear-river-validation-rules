﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PriceAggregateRootActor : AggregateRootActor<Price>
    {
        public PriceAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Price> bulkRepository,
            IBulkRepository<AdvertisementAmountRestriction> advertisementAmountRestrictionBulkRepository,
            IBulkRepository<AssociatedPositionGroupOvercount> associatedPositionGroupOvercountRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PriceAccessor(query), bulkRepository,
                HasValueObject(new AdvertisementAmountRestrictionAccessor(query), advertisementAmountRestrictionBulkRepository),
                HasValueObject(new AssociatedPositionGroupOvercountAccessor(query), associatedPositionGroupOvercountRepository));
        }

        public sealed class PriceAccessor : DataChangesHandler<Price>, IStorageBasedDataObjectAccessor<Price>
        {
            private readonly IQuery _query;

            public PriceAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionsGroupCount,
                    };

            public IQueryable<Price> GetSource()
                => _query.For<Facts::Price>().Select(price => new Price { Id = price.Id, BeginDate = price.BeginDate });

            public FindSpecification<Price> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Price>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AdvertisementAmountRestrictionAccessor : DataChangesHandler<AdvertisementAmountRestriction>, IStorageBasedDataObjectAccessor<AdvertisementAmountRestriction>
        {
            private readonly IQuery _query;

            public AdvertisementAmountRestrictionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified,
                        MessageTypeCode.MinimumAdvertisementAmount
                    };

            public IQueryable<AdvertisementAmountRestriction> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted)
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted).Where(x => x.IsControlledByAmount) on pricePosition.PositionId equals position.Id
                   group new { pricePosition.PriceId, position.CategoryCode, position.Name, pricePosition.MinAdvertisementAmount, pricePosition.MaxAdvertisementAmount }
                       by new { pricePosition.PriceId, position.CategoryCode } into groups
                   select new AdvertisementAmountRestriction
                       {
                           PriceId = groups.Key.PriceId,
                           CategoryCode = groups.Key.CategoryCode,
                           CategoryName = (from pp in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted).Where(x => x.PriceId == groups.Key.PriceId)
                                           join p in _query.For<Facts::Position>().Where(x => !x.IsDeleted).Where(x => x.IsControlledByAmount && x.CategoryCode == groups.Key.CategoryCode).OrderBy(x => x.Id) on pp.PositionId
                                               equals p.Id
                                           select p.Name).First(), // Этот кусок кода достаточно точно отражает текущее поведение в erm, решение лучше - создать справочник и слушать поток flowNomenclatures.NomenclatureCategory
                           Max = groups.Min(x => x.MaxAdvertisementAmount) ?? int.MaxValue,
                           Min = groups.Max(x => x.MinAdvertisementAmount) ?? 0,
                           MissingMinimalRestriction = groups.Max(x => x.MinAdvertisementAmount) == null
                       };

            public FindSpecification<AdvertisementAmountRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
            }
        }

        public sealed class AssociatedPositionGroupOvercountAccessor : DataChangesHandler<AssociatedPositionGroupOvercount>, IStorageBasedDataObjectAccessor<AssociatedPositionGroupOvercount>
        {
            // Предполагается, что когда начнём создавать события на втором этапе - события этого класса будут приводить к вызову одной соответствующей проверки
            private readonly IQuery _query;

            public AssociatedPositionGroupOvercountAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionsGroupCount
                    };

            public IQueryable<AssociatedPositionGroupOvercount> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted)
                   let count = _query.For<Facts::AssociatedPositionsGroup>().Count(x => x.PricePositionId == pricePosition.Id)
                   let name = _query.For<Facts::Position>().Where(x => !x.IsDeleted).Single(x => x.Id == pricePosition.PositionId).Name
                   where count > 1
                   select new AssociatedPositionGroupOvercount
                   {
                       PriceId = pricePosition.PriceId,
                       PricePositionId = pricePosition.Id,
                       PricePositionName = name,
                       Count = count,
                   };

            public FindSpecification<AssociatedPositionGroupOvercount> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AssociatedPositionGroupOvercount>(x => aggregateIds.Contains(x.PriceId));
            }
        }
    }
}