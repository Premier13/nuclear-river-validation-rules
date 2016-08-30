using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �������������� �������
    /// </summary>
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        [Obsolete("������, ����� ����������� ��������� ������ ��� ������ �������, ��� ������ ������ ��� ���� � ��������")]
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}