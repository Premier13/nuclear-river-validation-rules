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
        public void Apply(Cache cache)
            => cache.Entity<ConsumerState>()
                .HasKey(x => new {x.Topic, x.Partition});

        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<ConsumerState>()
                .HasSchemaName(Schema.ServiceSchema)
                .HasPrimaryKey(x => new {x.Topic, x.Partition});

        public void Apply(Writer writer)
            => writer.Entity<ConsumerState>()
                .HasRelationsProvider(this)
                .HasKey(x => new {x.Topic, x.Partition});

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection, IQueryable<ConsumerState> actual, IQueryable<ConsumerState> outdated)
            => Array.Empty<RelationRecord>();
    }
}
