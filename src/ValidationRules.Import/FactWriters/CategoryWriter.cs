using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactWriters
{
    /// <summary>
    /// Используется для актуализации рубрик.
    /// </summary>
    public sealed class CategoryWriter : IEntityWriter
    {
        private readonly IRelationProvider<Category> _relationProvider;

        public CategoryWriter(IRelationProvider<Category> relationProvider)
        {
            _relationProvider = relationProvider;
        }

        public IReadOnlyCollection<RelationRecord> Write(DataConnection dataConnection, ICollection data)
            => Write(dataConnection, (IReadOnlyCollection<Category>) data);

        private IReadOnlyCollection<RelationRecord> Write(
            DataConnection dataConnection, IReadOnlyCollection<Category> data)
        {
            var (upserted, deleted) = Instrumentation.Do(
                () => DeduplicateData(data), "Writer deduplicate", typeof(Category).Name);
            var (upsertedTable, deletedTable) = Instrumentation.Do(
                () => CreateDataTable(dataConnection, upserted, deleted), "Writer bulkcopy", typeof(Category).Name);
            var relations = Instrumentation.Do(
                () => CalculateRelations(dataConnection, upsertedTable, deletedTable), "Writer relations", typeof(Category).Name);
            Instrumentation.Do(
                () => WriteChanges(dataConnection, upsertedTable, deletedTable), "Writer write", typeof(Category).Name);

            return relations;
        }

        private (IReadOnlyCollection<Category>, IReadOnlyCollection<Category>) DeduplicateData(
            IReadOnlyCollection<Category> data)
        {
            var updated = new Dictionary<long, Category>();
            var deleted = new Dictionary<long, Category>();

            foreach (var category in data)
            {
                if (category.IsDeleted)
                {
                    deleted[category.Id] = category;
                }
                else
                {
                    // Если после удаления пришла информация об изменениях - наверно, рубрику восстановили или создали новую с прежним id.
                    deleted.Remove(category.Id);
                    updated[category.Id] = category;
                }
            }

            return (updated.Values, deleted.Values);
        }

        private (ITable<Category>, ITable<Category>) CreateDataTable(
            DataConnection dataConnection,
            IEnumerable<Category> upserted,
            IEnumerable<Category> deleted)
        {
            var updatedTable = dataConnection.CreateTable<Category>($"#{typeof(Category).Name}Update");
            updatedTable.BulkCopy(upserted);

            var inferredFirstLevel =
                from thirdLevelCategory in updatedTable.Where(x => x.L1Id == null)
                from updatedSecondLevelCategory in updatedTable.InnerJoin(x => x.Id == thirdLevelCategory.L2Id).DefaultIfEmpty()
                from storedSecondLevelCategory in dataConnection.GetTable<Category>().InnerJoin(x => x.Id == thirdLevelCategory.L2Id).DefaultIfEmpty()
                select new {thirdLevelCategory.Id, L3Id = updatedSecondLevelCategory.L1Id ?? storedSecondLevelCategory.L1Id };

            inferredFirstLevel.MergeInto(updatedTable)
                .On(x => x.Id, x => x.Id)
                .UpdateWhenMatched((target, source) => new Category {L1Id = source.L3Id})
                .Merge();

            var deletedTable = dataConnection.CreateTable<Category>($"#{typeof(Category).Name}Delete");
            deletedTable.BulkCopy(deleted);

            return (updatedTable, deletedTable);
        }

        private IReadOnlyCollection<RelationRecord> CalculateRelations(
            DataConnection dataConnection,
            IQueryable<Category> upserted,
            IQueryable<Category> deleted)
        {
            if (_relationProvider == null)
                return Array.Empty<RelationRecord>();

            var actualUpserted = upserted
                .LeftJoin(
                    dataConnection.GetTable<Category>(),
                    (x, y) => x.Id == y.Id,
                    (received, stored) => new { received, stored })
                .Where(x => x.received.L1Id != x.stored.L1Id || x.received.L2Id != x.stored.L2Id || x.received.L3Id != x.stored.L3Id);

            var actualDeleted = deleted
                .LeftJoin(
                    dataConnection.GetTable<Category>(),
                    (x, y) => x.Id == y.Id,
                    (received, stored) => new { received, stored })
                .Where(x => x.stored.IsDeleted != x.received.IsDeleted);

            var relations = _relationProvider.GetRelations(
                dataConnection,
                actualUpserted.Select(x => x.received).Union(actualDeleted.Select(x => x.received)),
                actualUpserted.Select(x => x.stored).Union(actualDeleted.Select(x => x.stored)));

            return relations;
        }

        private void WriteChanges(
            DataConnection dataConnection,
            ITable<Category> upserted,
            ITable<Category> deleted)
        {
            upserted.MergeInto(dataConnection.GetTable<Category>())
                .OnTargetKey()
                .UpdateWhenMatched()
                .InsertWhenNotMatched()
                .Merge();

            deleted.MergeInto(dataConnection.GetTable<Category>())
                .OnTargetKey()
                .UpdateWhenMatched((source, target) => new Category {IsDeleted = true})
                // .InsertWhenNotMatched()
                .Merge();
        }
    }
}