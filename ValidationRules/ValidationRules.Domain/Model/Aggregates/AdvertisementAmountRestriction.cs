namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ����� �����-����� � ������������� ��������, ������������� �� ERM
    /// </summary>
    public class AdvertisementAmountRestriction
    {
        public long PriceId { get; set; }
        public long PositionId { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}