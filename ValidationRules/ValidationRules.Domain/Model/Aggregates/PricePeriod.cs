using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ������ �������� �����-�����.
    /// ����� ���� ��������� ������ �����-����� ������ ���� ����������� � ��������� �������� �������� �����-�����
    /// </summary>
    public sealed class PricePeriod : IAggregateValueObject
    {
        public long PriceId { get; set; }
        public long PeriodId { get; set; }
    }
}