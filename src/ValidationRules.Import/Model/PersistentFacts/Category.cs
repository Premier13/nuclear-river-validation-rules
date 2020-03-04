namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class Category
    {
        public long Id { get; set; }

        public long? L1Id { get; set; }
        public long? L2Id { get; set; }
        public long? L3Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
