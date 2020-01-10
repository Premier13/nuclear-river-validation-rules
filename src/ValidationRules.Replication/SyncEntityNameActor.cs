﻿using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Facts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Replication
{
    public sealed class SyncEntityNameActor : IActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<EntityName> _bulkRepository;
        private readonly IEqualityComparer<EntityName> _identityComparer;
        private readonly IEqualityComparer<EntityName> _completeComparer;

        public SyncEntityNameActor(IQuery query,
                                   IBulkRepository<EntityName> bulkRepository,
                                   IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<EntityName>();
            _completeComparer = equalityComparerFactory.CreateCompleteComparer<EntityName>();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // db accessors
            var syncDataObjectCommands = commands.OfType<ISyncDataObjectCommand>().ToList();
            if (syncDataObjectCommands.Count != 0)
            {
                foreach (var accessor in CreateAccessors(_query))
                {
                    var specification = accessor.GetFindSpecification(syncDataObjectCommands);

                    var dataChangesDetector = new TwoPhaseDataChangesDetector<EntityName>(
                        spec => accessor.GetSource().WhereMatched(spec),
                        spec => _query.For<EntityName>().WhereMatched(spec),
                        _identityComparer,
                        _completeComparer);

                    var changes = dataChangesDetector.DetectChanges(specification);
                    _bulkRepository.Delete(changes.Complement);
                    _bulkRepository.Create(changes.Difference);
                    _bulkRepository.Update(changes.Intersection);
                }
            }

            // memory accessors
            var replaceDataObjectCommands = commands.OfType<IReplaceDataObjectCommand>().ToList();
            if (replaceDataObjectCommands.Count != 0)
            {
                foreach (var accessor in CreateMemoryAccessors())
                {
                    var specification = accessor.GetFindSpecification(replaceDataObjectCommands);

                    var dataChangesDetector = new TwoPhaseDataChangesDetector<EntityName>(
                        _ => accessor.GetDataObjects(replaceDataObjectCommands),
                        spec => _query.For<EntityName>().WhereMatched(spec),
                        _identityComparer,
                        _completeComparer);

                    var changes = dataChangesDetector.DetectChanges(specification);
                    _bulkRepository.Delete(changes.Complement);
                    _bulkRepository.Create(changes.Difference);
                    _bulkRepository.Update(changes.Intersection);
                }
            }

            return Array.Empty<IEvent>();
        }

        private static IEnumerable<IStorageBasedDataObjectAccessor<EntityName>> CreateAccessors(IQuery query)
        {
            return new IStorageBasedDataObjectAccessor<EntityName>[]
            {
                new CategoryNameAccessor(query),
                new FirmNameAccessor(query),
                new FirmAddressNameAccessor(query),
                new LegalPersonProfileNameAccessor(query),
                new OrderNameAccessor(query),
                new PositionNameAccessor(query),
                new ProjectNameAccessor(query),
                new ThemeNameAccessor(query),
                new NomenclatureCategoryNameAccessor(query),
            };
        }

        private static IEnumerable<IMemoryBasedDataObjectAccessor<EntityName>> CreateMemoryAccessors()
        {
            return new[]
            {
                new AdvertisementNameAccessor(null),
            };
        }

        public sealed class CategoryNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public CategoryNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Category)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Category,
                    Name = x.Name
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Category),
                    EntityTypeIds.Category);
        }

        public sealed class FirmNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public FirmNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Firm.All)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Firm,
                    Name = x.Name
                }); 

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Firm),
                    EntityTypeIds.Firm);
        }

        public sealed class FirmAddressNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public FirmAddressNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.FirmAddress.All)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.FirmAddress,
                    Name = x.Address
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(FirmAddress),
                    EntityTypeIds.FirmAddress);
        }

        public sealed class LegalPersonProfileNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public LegalPersonProfileNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.LegalPersonProfile)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.LegalPersonProfile,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(LegalPersonProfile),
                    EntityTypeIds.LegalPersonProfile);
        }

        public sealed class OrderNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public OrderNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Order)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Order,
                    Name = x.Number
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Order),
                    EntityTypeIds.Order);
        }

        public sealed class PositionNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public PositionNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Position)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Position,
                    Name = x.Name
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Position),
                    EntityTypeIds.Position);
        }

        public sealed class ProjectNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public ProjectNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Project)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Project,
                    Name = x.DisplayName
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Project),
                    EntityTypeIds.Project);
        }

        public sealed class ThemeNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public ThemeNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Theme)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Theme,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Theme),
                    EntityTypeIds.Theme);
        }

        public sealed class NomenclatureCategoryNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public NomenclatureCategoryNameAccessor(IQuery query) => _query = query;

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.NomenclatureCategory)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.NomenclatureCategory,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(NomenclatureCategory),
                    EntityTypeIds.NomenclatureCategory);
        }

        public sealed class AdvertisementNameAccessor : IMemoryBasedDataObjectAccessor<EntityName>
        {
            // ReSharper disable once UnusedParameter.Local
            public AdvertisementNameAccessor(IQuery query) { }

            public IReadOnlyCollection<EntityName> GetDataObjects(IEnumerable<ICommand> commands)
            {
                var dtos = commands.Cast<ReplaceDataObjectCommand>().SelectMany(x => x.Dtos).Cast<AdvertisementDto>();

                return dtos.Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Advertisement,
                    Name = x.Name
                }).ToList();
            }

            public FindSpecification<EntityName> GetFindSpecification(IEnumerable<ICommand> commands)
            {
                var ids = commands.Cast<ReplaceDataObjectCommand>().SelectMany(x => x.Dtos).Cast<AdvertisementDto>().Select(x => x.Id).ToHashSet();

                return new FindSpecification<EntityName>(x => x.EntityType == EntityTypeIds.Advertisement && ids.Contains(x.Id));
            }
        }

        private static FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands, Type type, int typeId)
        {
            var ids = commands.Cast<SyncDataObjectCommand>()
                              .Where(c => c.DataObjectType == type)
                              .SelectMany(x => x.DataObjectIds)
                              .ToHashSet()
                              .Select(id => new { Id = id, EntityType = typeId }).ToList();

            return SpecificationFactory<EntityName>.Contains(x => new { x.Id, x.EntityType }, ids);
        }
    }
}