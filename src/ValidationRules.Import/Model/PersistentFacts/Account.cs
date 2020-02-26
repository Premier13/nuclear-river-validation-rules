namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class Account
    {
        public long Id { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public long LegalPersonId { get; set; }
        public decimal Balance { get; set; }
    }
}
