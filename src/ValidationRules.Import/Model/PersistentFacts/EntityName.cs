namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class EntityName
    {
        public long Id { get; set; }
        public int EntityType { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
    }
}
