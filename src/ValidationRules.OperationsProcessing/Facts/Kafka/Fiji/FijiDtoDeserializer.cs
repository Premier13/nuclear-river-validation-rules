using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Confluent.Kafka;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka.Fiji
{
    public sealed class FijiDtoDeserializer : IDeserializer<ConsumeResult<Ignore, byte[]>, IFijiDto>
    {
        public IEnumerable<IFijiDto> Deserialize(IEnumerable<ConsumeResult<Ignore, byte[]>> messages)
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
                        "BUILDING" => xElements.Select(ParseBuilding),
                        _ => Array.Empty<IFijiDto>(),
                    };

                    return dtos;
                })
                // фильтр на заведомо ненужные dto
                .Where(x => x != null);
            
            return deserialized;
        }

        private static IFijiDto ParseBuilding(XElement xml)
        {
            var isDeleted = (bool?) xml.Attribute("IsDeleted") ?? false;
            if (isDeleted)
            {
                return new BuildingBulkDeleteDto
                {
                    Id = (long) xml.Attribute("Code"),
                };
            }
                
            return new BuildingDto
            {
                Id = (long) xml.Attribute("Code"),
                PurposeCode = (int) xml.Attribute("PurposeCode"),
            };
        }
    }
}