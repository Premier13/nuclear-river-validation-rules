﻿using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class FirmAddress
    {
        /// <summary>
        /// https://confluence.2gis.ru/pages/viewpage.action?pageId=245956763
        /// </summary>
        public static readonly IReadOnlyCollection<int> InvalidBuildingPurposeCodesForPoi = new[]
        {
            54,     // Метро (Россия, Украина, Казахстан, Кыргызстан)
            69,     // Наружная реклама (Россия, Украина, Казахстан, Кыргызстан)
            27,     // Религиозное сооружение (Россия, Украина, Казахстан, Кыргызстан)
            22,     // Памятник (Россия, Украина, Казахстан, Кыргызстан)
            510,    // Мечеть (ОАЭ)
            513,    // Религиозное сооружение (ОАЭ)
            517,    // Вход в метро (ОАЭ)
            302,    // Религиозное сооружение (Прага)
            317,    // Вход в метро (Прага)
            409,    // Религиозное сооружение (Сантьяго)
            403,    // Вход в метро (Сантьяго)
        };

        public long Id { get; set; }
        public long FirmId { get; set; }

        public bool IsLocatedOnTheMap { get; set; }

        public long? EntranceCode { get; set; }
        
        public long? BuildingId { get; set; }
    }

    public sealed class FirmAddressInactive
    {
        public long Id { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosedForAscertainment { get; set; }
    }
}