namespace NuClear.ValidationRules.Import.Model.Facts
{
    // fixme: временная копипаста из NuClear.ValidationRules.Storage.Model.Events
    public sealed class OrderConsistency
    {
        public long Id { get; set; }
        public long? LegalPersonId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long? BargainId { get; set; }
    }
}
