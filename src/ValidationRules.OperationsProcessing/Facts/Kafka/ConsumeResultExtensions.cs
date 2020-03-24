using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Confluent.Kafka;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.Kafka
{
    internal static class ConsumeResultExtensions
    {
        public static string GetMessageType(this ConsumeResult<Ignore, byte[]> result)
        {
            var sb = new StringBuilder();

            var beginLetter = false;
            foreach (var @byte in result.Value)
            {
                var @char = (char) @byte;

                if (!beginLetter)
                {
                    // начали буквой
                    if (char.IsLetter(@char))
                    {
                        beginLetter = true;
                        sb.Append(@char);
                    }
                }
                else
                {
                    // продолжаем буквой или цифрой
                    if (char.IsLetter(@char) || char.IsNumber(@char))
                    {
                        sb.Append(@char);
                    }
                    else
                    {
                        // заканчиваем когда пошло что-то другое
                        break;
                    }
                }
            }

            return sb.ToString();
        }
        
        public static XElement GetXElement(this ConsumeResult<Ignore, byte[]> result)
        {
            using var memoryStream = new MemoryStream(result.Value);
            using var streamReader = new StreamReader(memoryStream);
            using var xmlReader = XmlReader.Create(memoryStream);
            var xElement = XElement.Load(xmlReader);
            return xElement;
        }
    }
}