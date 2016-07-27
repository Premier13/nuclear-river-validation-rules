namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ������������ ���������� ����� AssociatedPositionsGroup ��� PricePosition (�������������� �� ������ �����)
    /// </summary>
    public sealed class AssociatedPositionGroupOvercount
    {
        public long PriceId { get; set; }
        public long PricePositionId { get; set; }
        public string PricePositionName { get; set; }
        public int Count { get; set; }
    }
}