namespace NuClear.ValidationRules.Import.Model.Service
{
    // fixme: временная копипаста из NuClear.ValidationRules.Storage.Model.Events
    public sealed class EventRecord
    {
        public long Id { get; set; }
        public string Flow { get; set; }
        public string Content { get; set; }
    }
}
