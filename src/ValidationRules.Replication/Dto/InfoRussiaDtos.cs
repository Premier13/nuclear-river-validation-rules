using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Dto
{
    public interface IInfoRussiaDto{}
    
    public sealed class FirmDto : IInfoRussiaDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
    
    public sealed class FirmAddressDto : IInfoRussiaDto
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocatedOnTheMap { get; set; }
        public long? EntranceCode { get; set; }
        public long? BuildingId { get; set; }
        
        public IEnumerable<int> Categories { get; set; }
    }
}