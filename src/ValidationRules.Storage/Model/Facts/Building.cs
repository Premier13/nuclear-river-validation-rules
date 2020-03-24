namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Building
    {
        public long Id { get; set; }
        public int PurposeCode { get; set; }
    }

    // фейковый тип, нужен для удаления
    public sealed class BuildingBulkDelete
    {
        public long Id { get; set; }
    }
}