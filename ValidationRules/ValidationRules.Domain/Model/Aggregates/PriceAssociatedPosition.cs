using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// �������� �������, �������� � ������.
    /// </summary>
    public sealed class PriceAssociatedPosition : IAggregateValueObject
    {
        public long PriceId { get; set; }

        public long AssociatedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }

        /// <summary>
        /// ������� ������. � ������ ������ ������ ���� � ������� ���� �� ���� PrincipalPositionId.
        /// </summary>
        public long GroupId { get; set; }
    }
}