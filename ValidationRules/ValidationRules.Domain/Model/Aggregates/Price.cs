using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �����-�����
    /// </summary>
    public sealed class Price : IAggregateRoot
    {
        public long Id { get; set; }
    }
}