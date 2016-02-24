using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ����� ������ � �����-������, ���������� �� ������ ������� ������ �� ERM
    /// </summary>
    public sealed class OrderPrice : IAggregateValueObject
    {
        public long OrderId { get; set; }
        public long PriceId { get; set; }
    }
}