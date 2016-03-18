using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �������������� �������
    /// </summary>
    public sealed class Position : IAggregateRoot
    {
        public long Id { get; set; }
        public long PositionCategoryId { get; set; }
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}