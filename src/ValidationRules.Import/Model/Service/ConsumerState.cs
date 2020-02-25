namespace NuClear.ValidationRules.Import.Model.Service
{
    public class ConsumerState
    {
        public string Topic { get; set; }
        public int Partition { get; set; }
        public long Offset { get; set; }
    }
}
