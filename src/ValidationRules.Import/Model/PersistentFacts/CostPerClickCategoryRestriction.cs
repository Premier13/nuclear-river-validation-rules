using System;

namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class CostPerClickCategoryRestriction
    {
        public long ProjectId { get; set; }
        public DateTime Start { get; set; }
        
        public long CategoryId { get; set; }
        public decimal MinCostPerClick { get; set; }
    }
}