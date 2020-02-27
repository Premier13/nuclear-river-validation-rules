namespace NuClear.ValidationRules.Import.Model.Facts
{
    public sealed class QuantitativeRule
    {
        public long RulesetId { get; set; }
        public long NomenclatureCategoryCode { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}