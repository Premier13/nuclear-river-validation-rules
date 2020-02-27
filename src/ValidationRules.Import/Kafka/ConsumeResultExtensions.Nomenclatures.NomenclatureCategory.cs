using System.Collections;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.Service;
using NomenclatureCategory = NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureCategory.NomenclatureCategory;
using PersistentFacts = NuClear.ValidationRules.Import.Model.PersistentFacts;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult,
            NomenclatureCategory nomenclatureCategory)
        {
            yield return new PersistentFacts.NomenclatureCategory
            {
                Id = nomenclatureCategory.Code,
            };

            yield return new ConsumerState
            {
                Topic = consumeResult.Topic,
                Partition = consumeResult.Partition,
                Offset = consumeResult.Offset
            };
        }
    }
}
