namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ����� ������ � �����-������, ���������� �� ������ ������� ������ �� ERM
    /// </summary>
    public sealed class OrderPrice
    {
        public long OrderId { get; set; }
        public long PriceId { get; set; }
    }
}