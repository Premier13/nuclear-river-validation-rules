using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ������ ������.
    /// ����� ���� �������� ������ ������ ���� ���������� � ��������� � �������� ���������� ������.
    /// </summary>
    public sealed class OrderPeriod : IAggregateValueObject
    {
        public long OrderId { get; set; }
        public long PeriodId { get; set; }
    }
}