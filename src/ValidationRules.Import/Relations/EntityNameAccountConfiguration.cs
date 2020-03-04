using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class EntityNameConfiguration : IEntityConfiguration, IRelationProvider<EntityName>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<EntityName>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => new {x.Id, x.TenantId});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<EntityName>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => new {x.Id, x.TenantId});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<EntityName> actual, IQueryable<EntityName> outdated)
            => Array.Empty<RelationRecord>();
    }
}
