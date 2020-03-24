using NuClear.Replication.Core;
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
using NuClear.Replication.Core.Tenancy;

namespace NuClear.ValidationRules.Replication
{
    public sealed class SyncEntityNameActor : IActor
    {
        private readonly IQuery _query;
        private readonly ITenantProvider _tenantProvider;
        private readonly IBulkRepository<EntityName> _bulkRepository;
        private readonly TwoPhaseDataChangesDetector<EntityName> _dataChangesDetector;

        public SyncEntityNameActor(IQuery query,
                                   ITenantProvider tenantProvider,
                                   IBulkRepository<EntityName> bulkRepository,
                                   IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _tenantProvider = tenantProvider;
            _bulkRepository = bulkRepository;
            _dataChangesDetector = new TwoPhaseDataChangesDetector<EntityName>(equalityComparerFactory);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // db accessors
            var syncDataObjectCommands = commands.OfType<ISyncDataObjectCommand>().ToList();
            if (syncDataObjectCommands.Count != 0)
            {
                foreach (var accessor in CreateAccessors(_query).Select(AddTenancy))
                {
                    var specification = accessor.GetFindSpecification(syncDataObjectCommands);

                    var source = accessor.GetSource().WhereMatched(specification);
                    var target = _query.For<EntityName>()
                        .Where(new FindSpecification<EntityName>(x => x.TenantId == _tenantProvider.Current))
                        .WhereMatched(specification);
                    var changes = _dataChangesDetector.DetectChanges(source, target);

                    _bulkRepository.Delete(changes.Complement);
                    _bulkRepository.Create(changes.Difference);
                    _bulkRepository.Update(changes.Intersection);
                }
            }

            // memory accessors
            var syncInMemoryDataObjectCommands = commands.OfType<ISyncInMemoryDataObjectCommand>().ToList();
            if (syncInMemoryDataObjectCommands.Count != 0)
            {
                foreach (var accessor in CreateMemoryAccessors())
                {
                    var specification = accessor.GetFindSpecification(syncInMemoryDataObjectCommands);

                    var source = accessor.GetDataObjects(syncInMemoryDataObjectCommands);
                    var target = _query.For<EntityName>().WhereMatched(specification);

                    var changes = _dataChangesDetector.DetectChanges(source, target);
                    _bulkRepository.Delete(changes.Complement);
                    _bulkRepository.Create(changes.Difference);
                    _bulkRepository.Update(changes.Intersection);
                }
            }

            return Array.Empty<IEvent>();
        }

        private IStorageBasedDataObjectAccessor<EntityName> AddTenancy(
            IStorageBasedDataObjectAccessor<EntityName> accessor) =>
            new TenantAccessor<EntityName>(accessor, _tenantProvider.Current);

        private static IEnumerable<IStorageBasedDataObjectAccessor<EntityName>> CreateAccessors(IQuery query)
        {
            return new IStorageBasedDataObjectAccessor<EntityName>[]
            {
                new CategoryNameAccessor(query),
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
            return new IMemoryBasedDataObjectAccessor<EntityName>[]
            {
                new AdvertisementNameAccessor(null),
                new FirmNameAccessor(null),
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
                var dtos = commands
                    .Cast<SyncInMemoryDataObjectCommand>()
                    .SelectMany(x => x.Dtos)
                    .OfType<AdvertisementDto>()
                    .GroupBy(x => x.Id)
                    .Select(x => x.Aggregate((a,b) => a.Offset > b.Offset ? a : b));

                return dtos.Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Advertisement,
                    Name = x.Name
                }).ToList();
            }

            public FindSpecification<EntityName> GetFindSpecification(IEnumerable<ICommand> commands)
            {
                var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<AdvertisementDto>().Select(x => x.Id).ToHashSet();

                return new FindSpecification<EntityName>(x => x.EntityType == EntityTypeIds.Advertisement && ids.Contains(x.Id));
            }
        }

        public sealed class FirmNameAccessor : IMemoryBasedDataObjectAccessor<EntityName>
        {
            // ReSharper disable once UnusedParameter.Local
            public FirmNameAccessor(IQuery query) { }

            public IReadOnlyCollection<EntityName> GetDataObjects(IEnumerable<ICommand> commands)
            {
                var dtos = commands
                    .Cast<SyncInMemoryDataObjectCommand>()
                    .SelectMany(x => x.Dtos)
                    .OfType<FirmDto>()
                    .GroupBy(x => x.Id)
                    .Select(x => x.Last());

                return dtos
                    .Where(Specs.Find.InfoRussia.Firm.All)
                    .Select(x => new EntityName
                {
                    Id = x.Id,
                    EntityType = EntityTypeIds.Firm,
                    Name = x.Name
                }).ToList();
            }

            public FindSpecification<EntityName> GetFindSpecification(IEnumerable<ICommand> commands)
            {
                var ids = commands.Cast<SyncInMemoryDataObjectCommand>().SelectMany(x => x.Dtos).OfType<FirmDto>().Select(x => x.Id).ToHashSet();
                return new FindSpecification<EntityName>(x => x.EntityType == EntityTypeIds.Firm && ids.Contains(x.Id));
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
