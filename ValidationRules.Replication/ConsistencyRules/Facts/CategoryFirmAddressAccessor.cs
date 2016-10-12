﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class CategoryFirmAddressAccessor : IStorageBasedDataObjectAccessor<CategoryFirmAddress>, IDataChangesHandler<CategoryFirmAddress>
    {
        private readonly IQuery _query;

        public CategoryFirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryFirmAddress> GetSource()
            => GetLevelThreeSource().Union(GetLevelOneSource());

        private IQueryable<CategoryFirmAddress> GetLevelThreeSource()
            => from cfa in _query.For<Erm::CategoryFirmAddress>()
               where cfa.IsActive && !cfa.IsDeleted
               select new CategoryFirmAddress
                   {
                       Id = cfa.Id,
                       CategoryId = cfa.CategoryId,
                       FirmAddressId = cfa.FirmAddressId,
                   };

        private IQueryable<CategoryFirmAddress> GetLevelOneSource()
            => from cfa in _query.For<Erm::CategoryFirmAddress>()
               from c3 in _query.For<Erm::Category>().Where(x => x.Id == cfa.CategoryId)
               from c2 in _query.For<Erm::Category>().Where(x => x.Id == c3.ParentId)
               where cfa.IsActive && !cfa.IsDeleted
               select new CategoryFirmAddress
               {
                   Id = cfa.Id,
                   CategoryId = c2.ParentId,
                   FirmAddressId = cfa.FirmAddressId,
               };


        public FindSpecification<CategoryFirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CategoryFirmAddress>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryFirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryFirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryFirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryFirmAddress> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.FirmAddressId).ToArray();

            var orderIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                from order in _query.For<Order>().Where(x => x.FirmId == firmAddress.FirmId)
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}