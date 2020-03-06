using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace NuClear.ValidationRules.Import.FactExtractors
{
    public interface IXmlFactExtractor
    {
        IEnumerable<object> Extract(XmlReader reader);
    }

    public abstract class XmlFactExtractorBase<T> : IXmlFactExtractor
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));

        public IEnumerable<object> Extract(XmlReader reader) =>
            Serializer.CanDeserialize(reader)
                ? Extract((T) Serializer.Deserialize(reader))
                : Enumerable.Empty<object>();

        protected abstract IEnumerable<object> Extract(T dto);
    }
}
