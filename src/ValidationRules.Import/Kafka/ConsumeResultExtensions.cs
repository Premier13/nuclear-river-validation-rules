using System.Collections;
using Confluent.Kafka;

using Account = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.Account.Account;
using LegalEntity = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalEntity.LegalEntity;
using LegalUnit = NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalUnit.LegalUnit;
using CpcInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.CpcInfo.CpcInfo;
using AdvModelInRubricInfo = NuClear.ValidationRules.Import.Model.CommonFormat.flowAdvModelsInfo.AdvModelInRubricInfo.AdvModelInRubricInfo;
using NomenclatureCategory = NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureCategory.NomenclatureCategory;
using NomenclatureElement = NuClear.ValidationRules.Import.Model.CommonFormat.flowNomenclatures.NomenclatureElement.NomenclatureElement;

namespace NuClear.ValidationRules.Import.Kafka
{
    public static partial class ConsumeResultExtensions
    {
        public static IEnumerable Transform(this ConsumeResult<Ignore, object> consumeResult) =>
            consumeResult?.Message?.Value switch
            {
                Account account => Transform(consumeResult, account),
                LegalEntity legalEntity => Transform(consumeResult, legalEntity),
                LegalUnit legalUnit => Transform(consumeResult, legalUnit),
                CpcInfo cpcInfo => Transform(consumeResult, cpcInfo),
                AdvModelInRubricInfo advModelInRubricInfo => Transform(consumeResult, advModelInRubricInfo),
                NomenclatureCategory nomenclatureCategory => Transform(consumeResult, nomenclatureCategory),
                NomenclatureElement nomenclatureElement => Transform(consumeResult, nomenclatureElement),
                _ => null,
            };
    }
}
