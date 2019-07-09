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
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>, IDataChangesHandler<Theme>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public ThemeAccessor(IQuery query) => _query = query;

        public IQueryable<Theme> GetSource() => _query
            .For(Specs.Find.Erm.Theme)
            .Select(x => new Theme
            {
                Id = x.Id,
                BeginDistribution = x.BeginDistribution,
                EndDistribution = x.EndDistribution + OneSecond,
                IsDefault = x.IsDefault,
            });

        public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
            return SpecificationFactory<Theme>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Theme> dataObjects)
            => new [] {new DataObjectCreatedEvent(typeof(Theme), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Theme> dataObjects)
            => new [] {new DataObjectUpdatedEvent(typeof(Theme), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Theme> dataObjects)
            => new [] {new DataObjectDeletedEvent(typeof(Theme), dataObjects.Select(x => x.Id))};

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Theme> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToHashSet();

            var projectIds =
                from themeOrgUnit in _query.For<ThemeOrganizationUnit>().Where(x => dataObjectIds.Contains(x.ThemeId))
                from project in _query.For<Project>().Where(x => x.OrganizationUnitId == themeOrgUnit.OrganizationUnitId)
                select project.Id;

            return new[] {new RelatedDataObjectOutdatedEvent(typeof(Theme), typeof(Project), projectIds.ToHashSet())};
        }
    }
}