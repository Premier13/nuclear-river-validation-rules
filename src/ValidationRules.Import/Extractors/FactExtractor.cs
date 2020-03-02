using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace NuClear.ValidationRules.Import.Extractors
{
    public sealed class FactExtractor : IFactExtractor
    {
        private readonly IReadOnlyCollection<IXmlFactExtractor> _extractors;

        public FactExtractor(IReadOnlyCollection<IXmlFactExtractor> extractors)
        {
            _extractors = extractors;
        }

        public IEnumerable<object> Extract(Stream stream)
        {
            using var reader = XmlReader.Create(stream);
            return _extractors.Aggregate(Enumerable.Empty<object>(),
                (objects, extractor) => objects.Concat(extractor.Extract(reader)));
        }
    }
}
