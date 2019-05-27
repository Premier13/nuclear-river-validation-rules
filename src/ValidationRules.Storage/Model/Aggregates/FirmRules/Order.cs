﻿using System;

namespace NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules
{
    public enum InvalidFirmState
    {
        NotSet = 0,
        Deleted,
        ClosedForever,
        ClosedForAscertainment
    }

    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public long Scope { get; set; }

        public sealed class FirmOrganiationUnitMismatch
        {
            public long OrderId { get; set; }
        }

        public sealed class PartnerPosition
        {
            public long OrderId { get; set; }
            public long DestinationFirmId { get; set; }
            public long DestinationFirmAddressId { get; set; }
        }

        public sealed class PremiumPartnerPosition
        {
            public long OrderId { get; set; }
        }

        public sealed class FmcgCutoutPosition
        {
            public long OrderId { get; set; }
        }

        public class InvalidFirm
        {
            public long OrderId { get; set; }
            public long FirmId { get; set; }
            public InvalidFirmState State { get; set; }
        }
    }
}