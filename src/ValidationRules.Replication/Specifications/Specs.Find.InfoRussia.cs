using System;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static class InfoRussia
            {
                public static class Firm
                {
                    public static Func<FirmDto, bool> Active => x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment;
                    public static Func<FirmDto, bool> Inactive => x => !(x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                    public static Func<FirmDto, bool> All => x => true; 
                }
                
                public static class FirmAddress
                {
                    public static Func<FirmAddressDto, bool> Active => x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment;
                    public static Func<FirmAddressDto, bool> Inactive => x => !(x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                    public static Func<FirmAddressDto, bool> All => x => true;
                }
            }
        }
    }
}