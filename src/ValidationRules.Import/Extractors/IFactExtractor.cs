using System.Collections.Generic;
using System.IO;

namespace NuClear.ValidationRules.Import.Extractors
{
    public interface IFactExtractor
    {
        IEnumerable<object> Extract(Stream stream);
    }
}
