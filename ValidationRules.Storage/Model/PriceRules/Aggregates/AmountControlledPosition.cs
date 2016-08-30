namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// ������������ ������� � ������,
    /// ����������� � ��������� �������������� �������, ���������� ������� ����� �����������������.
    /// 
    /// ������ ���������������, ����:
    ///     ���������� �������� IsControlledByAmount � �������������� ������� (�����������/��������� ������)
    ///     ���������� �������� CategoryCode � �������������� �������
    ///     ��������� OPA
    ///     ��������� OP
    /// </summary>
    public sealed class AmountControlledPosition
    {
        public long OrderId { get; set; }
        public long CategoryCode { get; set; }
    }
}