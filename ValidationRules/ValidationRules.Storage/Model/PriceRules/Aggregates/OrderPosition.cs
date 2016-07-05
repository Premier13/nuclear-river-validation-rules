namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ����� ������ � �������������� ��������, ������������� �� ERM.OrderPosition + ERM.OrderPositionAdv
    /// </summary>
    public sealed class OrderPosition
    {
        public long OrderId { get; set; }
        public long OrderPositionId { get; set; }
        public long PackagePositionId { get; set; }
        public long ItemPositionId { get; set; }

        public bool HasNoBinding { get; set; }
        public long? Category3Id { get; set; }
        public long? Category1Id { get; set; }
        public long? FirmAddressId { get; set; }
    }
}