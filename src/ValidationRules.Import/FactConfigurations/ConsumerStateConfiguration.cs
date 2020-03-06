using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.FactWriters;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactConfigurations
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
