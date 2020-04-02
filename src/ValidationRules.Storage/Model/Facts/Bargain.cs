using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Bargain
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public DateTime SignupDate { get; set; }
    }
}
