using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �����-�����
    /// </summary>
    public sealed class Price
    {
        public long Id { get; set; }
        public DateTime BeginDate { get; set; }
    }
}