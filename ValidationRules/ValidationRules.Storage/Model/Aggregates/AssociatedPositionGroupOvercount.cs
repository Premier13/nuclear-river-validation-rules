namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ������������ ���������� ����� AssociatedPositionsGroup ��� PricePosition (�������������� �� ������ �����)
    /// </summary>
    public sealed class AssociatedPositionGroupOvercount
    {
        public long PriceId { get; set; }
        public long PricePositionId { get; set; }
        public int Count { get; set; }
    }
}