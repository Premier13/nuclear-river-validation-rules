using System;

namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ������ ������.
    /// ����� ���� �������� ������ ������ ���� ���������� � ��������� � �������� ���������� ������.
    /// </summary>
    public sealed class OrderPeriod
    {
        public long OrderId { get; set; }
        public long ProjectId { get; set; }
        public DateTime Start { get; set; }
    }
}