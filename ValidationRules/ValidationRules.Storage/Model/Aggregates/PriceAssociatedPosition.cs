namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// �������� �������, �������� � ������.
    /// </summary>
    public sealed class PriceAssociatedPosition
    {
        // todo: ��� ������ ����, ����� ������� ��� ��������?
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