﻿using System;
using System.Collections.Generic;
using System.Linq;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.AccountRules;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.AdvertisementRules;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.PriceRules;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.ProjectRules;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.ConsistencyRules;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.ThemeRules;
using SystemAggregates = NuClear.ValidationRules.Storage.Model.Aggregates.SystemRules;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal static partial class EntityTypeMap
    {
        private static readonly Dictionary<(Type, Type), IReadOnlyCollection<Type>> AggregateEventMapping =
            new Dictionary<(Type, Type), IList<Type>>()
                // AccountAggregates
                .Aggregate<AccountAggregates::Account>(
                    x => x.Match<Facts::Account>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderConsistency>()
                          .DependOn<Facts::Bargain>()
                          .DependOn<Facts::AccountDetail>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::ReleaseWithdrawal>())
                .Aggregate<AccountAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderConsistency>()
                          .DependOn<Facts::Bargain>()
                          .DependOn<Facts::UnlimitedOrder>()
                          .DependOn<Facts::Account>())

                // AdvertisementAggregates
                .Aggregate<AdvertisementAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::Advertisement>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::PositionChild>()
                          .DependOn<Facts::PricePosition>())

                // ConsistencyAggregates
                .Aggregate<ConsistencyAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderConsistency>()
                          .DependOn<Facts::Bargain>()
                          .DependOn<Facts::BargainScanFile>()
                          .DependOn<Facts::Bill>()
                          .DependOn<Facts::BranchOffice>()
                          .DependOn<Facts::BranchOfficeOrganizationUnit>()
                          .DependOn<Facts::Deal>()
                          .DependOn<Facts::LegalPerson>()
                          .DependOn<Facts::LegalPersonProfile>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderScanFile>()
                          .DependOn<Facts::ReleaseWithdrawal>())

                // FirmAggregates
                .Aggregate<FirmAggregates::Firm>(
                    x => x.Match<Facts::Firm>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderItem>())
                .Aggregate<FirmAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::Firm>()
                          .DependOn<Facts::FirmInactive>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::FirmAddressInactive>()
                          .DependOn<Facts::FirmAddressCategory>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::PricePosition>())

                // PriceAggregates
                .Aggregate<PriceAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::Price>()
                          .DependOn<Facts::PricePosition>())
                .Aggregate<PriceAggregates::Period>(
                    x => x.Match<Facts::Project>()
                          .DependOn<Facts::Order>())
                .Aggregate<PriceAggregates::Firm>(
                    x => x.Match<Facts::Firm>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::OrderItem>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::Ruleset>())
                .Aggregate<PriceAggregates::Ruleset>(
                    x => x.Match<Facts::Ruleset>()
                          .DependOn<Facts::NomenclatureCategory>())

                // ProjectAggregates
                .Aggregate<ProjectAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderWorkflow>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionCostPerClick>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::PricePosition>())
                .Aggregate<ProjectAggregates::Project>(
                    x => x.Match<Facts::Project>()
                          .DependOn<Facts::CostPerClickCategoryRestriction>()
                          .DependOn<Facts::SalesModelCategoryRestriction>()
                          .DependOn<Facts::CategoryOrganizationUnit>()
                          .DependOn<Facts::ReleaseInfo>())

                // ThemeAggregates
                .Aggregate<ThemeAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>())
                .Aggregate<ThemeAggregates::Project>(
                    x => x.Match<Facts::Project>()
                          .DependOn<Facts::Theme>()
                          .DependOn<Facts::ThemeOrganizationUnit>())
                .Aggregate<ThemeAggregates::Theme>(
                    x => x.Match<Facts::Theme>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::ThemeCategory>())

                // SystemAggregates
                .Aggregate<SystemAggregates::SystemStatus>(
                    x => x.Match<Facts::SystemStatus>())

                .ToDictionary(x => x.Key, x => (IReadOnlyCollection<Type>)x.Value);

        public static bool TryGetAggregateTypes(Type factType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            var key = (factType, factType);
            return AggregateEventMapping.TryGetValue(key, out aggregateTypes);
        }

        public static bool TryGetRelatedAggregateTypes(Type factType, Type relatedFactType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            var key = (factType, relatedFactType);
            return AggregateEventMapping.TryGetValue(key, out aggregateTypes);
        }

        private static Dictionary<(Type, Type), IList<Type>> Aggregate<TAggregate>(
            this Dictionary<(Type, Type), IList<Type>> dictionary,
            Action<FluentDictionaryBuilder> action)
        {
            var builder = new FluentDictionaryBuilder();
            action.Invoke(builder);

            dictionary.Append((builder.Matched, builder.Matched), typeof(TAggregate));
            foreach (var depended in builder.Depended)
            {
                // Первым идёт тип, от которого зависит агрегат. Он же тип, accessor которого сгенерировал событие.
                dictionary.Append((depended, builder.Matched), typeof(TAggregate));
            }

            return dictionary;
        }

        private static void Append<TKey, TValue>(this Dictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                dictionary.Add(key, list);
            }

            list.Add(value);
        }

        private sealed class FluentDictionaryBuilder
        {
            public Type Matched { get; private set; }

            public IList<Type> Depended { get; } = new List<Type>();

            public FluentDictionaryBuilder Match<T>()
            {
                if (Matched != null)
                {
                    throw new InvalidOperationException("Matched has already been set");
                }

                Matched = typeof(T);
                return this;
            }

            // todo: информацию DependOn должно быть легко вытащить из выражений, тогда не нужно будет её поддерживать вручную
            public FluentDictionaryBuilder DependOn<T>()
            {
                Depended.Add(typeof(T));
                return this;
            }
        }

    }
}