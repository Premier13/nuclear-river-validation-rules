using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ������ ������.
    /// ����� ���� �������� ������ ������ ���� ���������� � ��������� � �������� ���������� ������.
    /// </summary>
    public sealed class OrderPeriod
    {
        public long OrderId { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
        public long Scope { get; set; }
    }
}