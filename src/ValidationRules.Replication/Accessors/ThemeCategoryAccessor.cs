﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class ThemeCategoryAccessor : IStorageBasedDataObjectAccessor<ThemeCategory>, IDataChangesHandler<ThemeCategory>
    {
        private readonly IQuery _query;

        public ThemeCategoryAccessor(IQuery query) => _query = query;

        public IQueryable<ThemeCategory> GetSource() => _query
            .For<Erm::ThemeCategory>()
            .Where(x => !x.IsDeleted)
            .Select(x => new ThemeCategory
            {
                Id = x.Id,
                ThemeId = x.ThemeId,
                CategoryId = x.CategoryId
            });

        public FindSpecification<ThemeCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<ThemeCategory>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<ThemeCategory> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ThemeCategory> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ThemeCategory> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ThemeCategory> dataObjects)
        {
            var themeIds = dataObjects.Select(x => x.ThemeId);

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(ThemeCategory), typeof(Theme), themeIds.ToHashSet())};
        }
    }
}