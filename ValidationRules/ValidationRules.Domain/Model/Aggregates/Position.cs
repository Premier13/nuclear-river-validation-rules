using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �������������� �������
    /// </summary>
    public class Position : IAggregateRoot
    {
        public long Id { get; set; }
        public long PositionCategoryId { get; set; }
    }
}