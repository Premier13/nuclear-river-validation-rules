namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// �������� ���� ����������� ���� � ����� �������.
    /// </summary>
    public sealed class PriceDeniedPosition
    {
        // todo: ���� � ������� ���� OrderDeniedPosition, �� ��� ������� ����� �������?
        public long PriceId { get; set; }

        public long DeniedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}