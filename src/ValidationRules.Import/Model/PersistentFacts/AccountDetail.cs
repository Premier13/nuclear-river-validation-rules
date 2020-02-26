using System;

namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public class AccountDetail
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public long AccountId { get; set; }
        public long OrderId { get; set; }
        public DateTime PeriodStartDate { get; set; }
    }
}
