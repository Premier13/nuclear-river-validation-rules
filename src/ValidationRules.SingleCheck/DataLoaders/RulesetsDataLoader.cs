﻿using System.Linq;

using LinqToDB.Data;
using NuClear.ValidationRules.SingleCheck.Store;

using Ruleset = NuClear.ValidationRules.Storage.Model.Facts.Ruleset;

namespace NuClear.ValidationRules.SingleCheck.DataLoaders
{
    public static class RulesetsDataLoader
    {
        public static void Load(ErmDataLoader.ResolvedOrderSummary orderSummary, DataConnection query, IStore store)
        {
            var rulesetDtos = query.GetTable<Ruleset>()
                                   .Where(r => !r.IsDeleted && r.BeginDate <= orderSummary.BeginDate && orderSummary.BeginDate < r.EndDate)
                                   .Join(query.GetTable<Ruleset.RulesetProject>(),
                                         ruleset => ruleset.Id,
                                         rulesetProject => rulesetProject.RulesetId,
                                         (ruleset, rulesetProject) => new
                                             {
                                                 Ruleset = ruleset,
                                                 RulesetProject = rulesetProject
                                             })
                                   .Where(x => x.RulesetProject.ProjectId == orderSummary.ProjectId)
                                   .Execute();

            var rulesets = rulesetDtos.Select(x => x.Ruleset).ToList();
            store.AddRange(rulesets);
            store.AddRange(rulesetDtos.Select(x => x.RulesetProject).ToList());

            var targetRulesetsIds = rulesets.Select(r => r.Id)
                                            .ToList();

            var usedNomenclatures = orderSummary.SoldPackagesIds
                                                .Union(orderSummary.SoldPackageElementsIds)
                                                .ToList();

            // Правила на сопутствие требуются только по PositionId или AssociatedPositionId, которые есть в заказе
            var associatedRulesForOrder = query.GetTable<Ruleset.AssociatedRule>()
                                               .Where(rule => targetRulesetsIds.Contains(rule.RulesetId)
                                                              && (orderSummary.SoldPackagesIds.Contains(rule.AssociatedNomenclatureId)
                                                                  || usedNomenclatures.Contains(rule.PrincipalNomenclatureId)))
                                               .Execute();
            store.AddRange(associatedRulesForOrder);

            // Правила на запрещение требуются только по PositionId, которые есть в заказе
            var deniedRules = query.GetTable<Ruleset.DeniedRule>()
                                   .Where(rule => targetRulesetsIds.Contains(rule.RulesetId)
                                                  && usedNomenclatures.Contains(rule.NomenclatureId)) // т.к. при импорте правил создаем симметричные, то можно фильтровать только по одной из номенклатур правила
                                   .Execute();
            store.AddRange(deniedRules);

            var quantitativeRules = query.GetTable<Ruleset.QuantitativeRule>()
                                   .Where(rule => targetRulesetsIds.Contains(rule.RulesetId)) // пока не фильтруем по categorycodes
                                   .Execute();
            store.AddRange(quantitativeRules);
        }
    }
}
