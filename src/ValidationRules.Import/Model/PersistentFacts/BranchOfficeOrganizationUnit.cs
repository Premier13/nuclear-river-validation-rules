namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public class BranchOfficeOrganizationUnit : IDeletable
    {
        public long Id { get; set; }
        public long BranchOfficeId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
