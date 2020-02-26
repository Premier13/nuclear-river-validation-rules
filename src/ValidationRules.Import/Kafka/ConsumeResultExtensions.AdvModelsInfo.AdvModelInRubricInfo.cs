using System.Collections;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;

using AdvModelInRubricInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.AdvModelInRubricInfo.AdvModelInRubricInfo;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult, AdvModelInRubricInfo advModelInRubricInfo)
        {
            foreach (var rubric in advModelInRubricInfo.AdvModelsInRubrics)
            {
                yield return new SalesModelCategoryRestriction
                {
                    ProjectId = advModelInRubricInfo.BranchCode,
                    Start = advModelInRubricInfo.BeginningDate,
                    CategoryId = rubric.Code,
                    SalesModel = (int)rubric.AdvModel,
                };
            }

            yield return new ConsumerState
            {
                Topic = consumeResult.Topic,
                Partition = consumeResult.Partition,
                Offset = consumeResult.Offset
            };
        }
    }
}
