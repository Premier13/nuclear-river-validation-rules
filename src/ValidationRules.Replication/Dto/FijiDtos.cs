namespace NuClear.ValidationRules.Replication.Dto
{
    public interface IFijiDto{}
    
    public sealed class BuildingDto : IFijiDto
    {
        public long Id { get; set; }
        public int PurposeCode { get; set; }
    }

    public sealed class BuildingBulkDeleteDto : IFijiDto
    {
        public long Id { get; set; }        
    } 
}