namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ������ ������.
    /// ����� ���� �������� ������ ������ ���� ���������� � ��������� � �������� ���������� ������.
    /// </summary>
    public class OrderPeriod
    {
        public long OrderId { get; set; }
        public long PeriodId { get; set; }
    }
}