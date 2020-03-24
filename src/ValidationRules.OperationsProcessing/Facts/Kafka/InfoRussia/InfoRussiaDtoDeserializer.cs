using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Confluent.Kafka;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.InfoRussia
{
    public sealed class InfoRussiaDtoDeserializer : IDeserializer<ConsumeResult<Ignore, byte[]>, IInfoRussiaDto>
    {
        public IEnumerable<IInfoRussiaDto> Deserialize(IEnumerable<ConsumeResult<Ignore, byte[]>> messages)
        {
            var filtered = messages.Where(x => x.Value != null);

            var deserialized = filtered
                .GroupBy(x => x.GetMessageType())
                .SelectMany(x =>
                {
                    var messageType = x.Key.ToUpperInvariant();
                    var xElements = x.Select(y => y.GetXElement());

                    var dtos = messageType switch
                    {
                        "FIRM" => xElements.Select(ParseFirm),
                        "CARD" => xElements.Select(ParseCard),
                        _ => Array.Empty<IInfoRussiaDto>(),
                    };

                    return dtos;
                })
                // фильтр на заведомо ненужные dto
                .Where(x => x != null);

            return deserialized;
        }

        private static IInfoRussiaDto ParseFirm(XElement xml)
        {
            var branchCode = (int?) xml.Attribute("BranchCode");
            if (branchCode == null)
            {
                // не импортируем фирмы без проектов
                return null;
            }

            var firmDto = new FirmDto
            {
                Id = (long) xml.Attribute("Code"),
                Name = (string) xml.Attribute("Name"),
                ProjectId = branchCode.Value,

                ClosedForAscertainment = (bool?) xml.Attribute("IsHidden") ?? false,
                IsActive = !((bool?) xml.Attribute("IsArchived") ?? false),
                IsDeleted = (bool?) xml.Attribute("IsDeleted") ?? false,
            };

            return firmDto;
        }

        private static IInfoRussiaDto ParseCard(XElement xml)
        {
            var firmAddressDto = new FirmAddressDto
            {
                Id = (long) xml.Attribute("Code"),
                FirmId = (long) xml.Attribute("FirmCode"),
                ClosedForAscertainment = (bool?) xml.Attribute("IsHidden") ?? false,
                IsActive = !((bool?) xml.Attribute("IsArchived") ?? false),
                IsDeleted = (bool?) xml.Attribute("IsDeleted") ?? false,
                IsLocatedOnTheMap = (bool?) xml.Attribute("IsLocal") ?? true,
                Categories = xml.Element("Rubrics")?.Elements("Rubric").Select(x => (int)x.Attribute("Code")) ?? Array.Empty<int>(),
            };

            var address = xml.Element("Address");
            if (address != null)
            {
                firmAddressDto.EntranceCode = (long?)address.Attribute("EntranceCode");
                firmAddressDto.BuildingId = (long?)address.Attribute("BuildingCode");
            }
            
            return firmAddressDto;
        }
    }
}