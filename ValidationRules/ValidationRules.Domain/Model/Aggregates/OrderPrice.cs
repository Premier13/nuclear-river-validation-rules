namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ����� ������ � �����-������, ���������� �� ������ ������� ������ �� ERM
    /// </summary>
    public class OrderPrice
    {
        public long OrderId { get; set; }
        public long PriceId { get; set; }
    }
}