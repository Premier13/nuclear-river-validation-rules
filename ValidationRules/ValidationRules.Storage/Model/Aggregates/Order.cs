namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� ������
    /// </summary>
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Number { get; set; }
    }
}