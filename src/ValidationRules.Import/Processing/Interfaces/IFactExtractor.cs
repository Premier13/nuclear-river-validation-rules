using System.Collections.Generic;
using System.IO;

namespace NuClear.ValidationRules.Import.Processing.Interfaces
{
    public interface IFactExtractor
    {
        IEnumerable<object> Extract(Stream stream);
    }
}
