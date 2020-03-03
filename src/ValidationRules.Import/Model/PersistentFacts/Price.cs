using System;

namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class Price : IDeletable
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public DateTime BeginDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
