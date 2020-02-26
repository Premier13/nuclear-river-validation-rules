using System.Collections;
using Confluent.Kafka;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;

using CpcInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.CpcInfo.CpcInfo;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        private static IEnumerable Transform(ConsumeResult<Ignore, object> consumeResult, CpcInfo cpcInfo)
        {
            foreach (var rubric in cpcInfo.Rubrics)
            {
                yield return new CostPerClickCategoryRestriction
                {
                    ProjectId = cpcInfo.BranchCode,
                    Start = cpcInfo.BeginningDate,
                    CategoryId = rubric.Code,
                    MinCostPerClick = (decimal)rubric.Cpc,
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
