namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ����� ������ � �������������� ��������, ������������� �� ERM.
    /// </summary>
    public class OrderPosition
    {
        public long OrderId { get; set; }
        public long PositionId { get; set; }
    }
}