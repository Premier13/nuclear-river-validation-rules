﻿using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class LegalPersonProfile
    {
        public long Id { get; set; }
        public long LegalPersonId { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}
