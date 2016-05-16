namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� �������������� �������
    /// </summary>
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}