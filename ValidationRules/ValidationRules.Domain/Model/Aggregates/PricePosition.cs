namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ����� �����-����� � ������������� ��������, ������������� �� ERM
    /// </summary>
    public class PricePosition
    {
        public long PriceId { get; set; }
        public long PositionId { get; set; }
    }
}