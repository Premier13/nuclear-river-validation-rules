using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NuClear.ValidationRules.Import.Processing.Interfaces;

namespace NuClear.ValidationRules.Import.FactExtractors
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
