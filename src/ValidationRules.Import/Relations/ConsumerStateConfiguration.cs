using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import.Relations
{
    public sealed class ConsumerStateConfiguration : IEntityConfiguration, IRelationProvider<ConsumerState>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<ConsumerState>()
                .HasSchemaName(Schema.ServiceSchema)
                .HasPrimaryKey(x => new {x.Topic, x.Partition});

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<ConsumerState>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => new {x.Topic, x.Partition});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<ConsumerState> actual, IQueryable<ConsumerState> outdated)
            => Array.Empty<RelationRecord>();
    }
}
