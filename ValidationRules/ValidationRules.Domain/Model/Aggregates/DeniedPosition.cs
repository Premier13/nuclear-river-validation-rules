namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// �������� ���� ����������� ���� � ����� �������.
    /// </summary>
    public sealed class DeniedPosition
    {
        public long PositionId { get; set; }
        public long DeniedPositionId { get; set; }

        /// <summary>
        /// ������������ ����, ���� null, �� ������� ��������� ��� ���� �����-������.
        /// </summary>
        public long? PriceId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}