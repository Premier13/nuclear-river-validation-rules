using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NuClear.ValidationRules.Import.FactWriters;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactConfigurations
{
    public sealed class NomenclatureCategoryConfiguration : IEntityConfiguration, IRelationProvider<NomenclatureCategory>
    {
        public void Apply(FluentMappingBuilder builder)
            => builder.Entity<NomenclatureCategory>()
                .HasSchemaName(Schema.PersistentFactsSchema)
                .HasPrimaryKey(x => x.Id);

        public void Apply(CacheSaver cacheSaver, bool enableRelations)
            => cacheSaver.Entity<NomenclatureCategory>()
                .HasRelationsProvider(enableRelations ? this : null)
                .HasKey(x => x.Id);

        public IReadOnlyCollection<RelationRecord> GetRelations(DataConnection dataConnection,
            IQueryable<NomenclatureCategory> actual, IQueryable<NomenclatureCategory> outdated)
        {
            // тип ни на что не влияет. он вообще в таком случае нужен?
            // ps. сам - может и не нужен, но его название - да.
            return Array.Empty<RelationRecord>();
        }
    }
}