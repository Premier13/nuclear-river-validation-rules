namespace NuClear.ValidationRules.Storage.Model.FirmRules.Facts
{
    public class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long? CategoryId { get; set; }
        public long PositionId { get; set; }
    }
}