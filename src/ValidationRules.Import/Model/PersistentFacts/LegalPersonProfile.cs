using System;

namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public class LegalPersonProfile
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public long LegalPersonId { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}
