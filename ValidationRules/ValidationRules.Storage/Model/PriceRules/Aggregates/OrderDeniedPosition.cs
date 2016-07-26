namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ������������ ������, ����������� ����� �� ������� ������.
    /// ������ ���������������� �� ������ �� ������� ������, � �������� �� ��������,
    /// �� � �� ���� ������ ����-�� ������� ���-�� �����.
    /// </summary>
    public sealed class OrderDeniedPosition : IBindingObject
    {
        public long OrderId { get; set; }
        public long CauseOrderPositionId { get; set; }
        public long CausePackagePositionId { get; set; }
        public long CauseItemPositionId { get; set; }

        public long DeniedPositionId { get; set; }
        public long BindingType { get; set; }

        public bool HasNoBinding { get; set; }
        public long? Category3Id { get; set; }
        public long? FirmAddressId { get; set; }
        public long? Category1Id { get; set; }

        public string Source { get; set; }
    }
}