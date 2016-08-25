using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ������ �������� �����-�����.
    /// ����� ���� ��������� ������ �����-����� ������ ���� ����������� � ��������� �������� �������� �����-�����
    /// </summary>
    public sealed class PricePeriod
    {
        public long PriceId { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
    }
}