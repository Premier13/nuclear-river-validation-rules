namespace NuClear.ValidationRules.Import.Model.PersistentFacts
{
    public sealed class Position
    {
        public long Id { get; set; }

        public int BindingObjectType { get; set; }
        public int SalesModel { get; set; }
        public int PositionsGroup { get; set; }

        public bool IsCompositionOptional { get; set; }
        public int ContentSales { get; set; }

        public bool IsControlledByAmount { get; set; }

        public long CategoryCode { get; set; }

        public bool IsDeleted { get; set; }
    }
}
