﻿namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class FirmCategoryForecast : IBitFactObject
    {
        public long FirmId { get; set; }
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
        public int ForecastClick { get; set; }
        public decimal ForecastAmount { get; set; }
    }
}