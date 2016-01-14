namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ������ �������� �����-�����.
    /// ����� ���� ��������� ������ �����-����� ������ ���� ����������� � ��������� �������� �������� �����-�����
    /// </summary>
    public class PricePeriod
    {
        public long PriceId { get; set; }
        public long PeriodId { get; set; }
    }
}