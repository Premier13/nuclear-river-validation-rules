﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Aggregates.ThemeRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class ProjectAggregateRootActor : AggregateRootActor<Project>
    {
        public ProjectAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Project> bulkRepository,
            IBulkRepository<Project.ProjectDefaultTheme> projectDefaultThemeBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new ProjectAccessor(query), bulkRepository,
               HasValueObject(new ProjectDefaultThemeAccessor(query), projectDefaultThemeBulkRepository));
        }

        public sealed class ProjectAccessor : DataChangesHandler<Project>, IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query) : base(CreateInvalidator()) => _query = query;

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DefaultThemeMustBeExactlyOne,
                    };

            public IQueryable<Project> GetSource()
                => from project in _query.For<Facts::Project>()
                   select new Project
                   {
                       Id = project.Id,
                   };

            public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<SyncDataObjectCommand>().SelectMany(c => c.DataObjectIds).ToHashSet();
                return new FindSpecification<Project>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class ProjectDefaultThemeAccessor : DataChangesHandler<Project.ProjectDefaultTheme>, IStorageBasedDataObjectAccessor<Project.ProjectDefaultTheme>
        {
            private readonly IQuery _query;

            public ProjectDefaultThemeAccessor(IQuery query) : base(CreateInvalidator()) => _query = query;

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DefaultThemeMustBeExactlyOne,
                    };

            public IQueryable<Project.ProjectDefaultTheme> GetSource()
            {
                var themeProjects =
                    from project in _query.For<Facts::Project>()
                    from themeOrgUnit in _query.For<Facts::ThemeOrganizationUnit>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                    from theme in _query.For<Facts::Theme>().Where(x => x.IsDefault).Where(x => x.Id == themeOrgUnit.ThemeId)
                    select new Project.ProjectDefaultTheme
                    {
                        ProjectId = project.Id,
                        ThemeId = themeOrgUnit.ThemeId,
                        Start = theme.BeginDistribution,
                        End = theme.EndDistribution,
                    };

                return themeProjects;
            }

            public FindSpecification<Project.ProjectDefaultTheme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().SelectMany(c => c.AggregateRootIds).ToHashSet();
                return new FindSpecification<Project.ProjectDefaultTheme>(x => aggregateIds.Contains(x.ProjectId));
            }
        }
    }
}
