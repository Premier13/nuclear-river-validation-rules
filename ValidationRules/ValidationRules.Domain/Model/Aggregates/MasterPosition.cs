namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// �������� �������, �������� � ������.
    /// </summary>
    public class MasterPosition
    {
        public long PositionId { get; set; }
        public long MasterPositionId { get; set; }

        /// <summary>
        /// ������������ ����, ���� null, �� ������� ��������� ��� ���� �����-������.
        /// </summary>
        public long? PriceId { get; set; }

        /// <summary>
        /// ������� ������. � ������ ������ ������ ���� � ������� ���� �� ���� MasterPositionId.
        /// </summary>
        public long? GroupId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}