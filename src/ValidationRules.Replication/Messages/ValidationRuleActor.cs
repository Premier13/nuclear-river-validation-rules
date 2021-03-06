﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Replication.AccountRules.Validation;
using NuClear.ValidationRules.Replication.AdvertisementRules.Validation;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.ConsistencyRules.Validation;
using NuClear.ValidationRules.Replication.FirmRules.Validation;
using NuClear.ValidationRules.Replication.PriceRules.Validation;
using NuClear.ValidationRules.Replication.ProjectRules.Validation;
using NuClear.ValidationRules.Replication.SystemRules.Validation;
using NuClear.ValidationRules.Replication.ThemeRules.Validation;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ValidationRuleActor : IActor
    {
        private readonly IQuery _query;
        private readonly IRepository<Version> _versionRepository;
        private readonly SyncInMemoryDataObjectsActor<Version.ErmState> _syncInMemoryErmStates;
        private readonly IRepository<Version.AmsState> _amsStatesRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;
        private readonly Dictionary<MessageTypeCode, IValidationResultAccessor> _accessors;
        private readonly IEqualityComparer<Version.ValidationResult> _equalityComparer;
        private readonly ValidationResultCache _cache;

        public ValidationRuleActor(IQuery query,
                                   IRepository<Version> versionRepository,
                                   SyncInMemoryDataObjectsActor<Version.ErmState> syncInMemoryErmStates,
                                   IRepository<Version.AmsState> amsStatesRepository,
                                   IBulkRepository<Version.ValidationResult> validationResultRepository,
                                   IEqualityComparerFactory equalityComparerFactory,
                                   ValidationResultCache cache)
        {
            _query = query;
            _versionRepository = versionRepository;
            _syncInMemoryErmStates = syncInMemoryErmStates;
            _validationResultRepository = validationResultRepository;
            _amsStatesRepository = amsStatesRepository;
            _accessors = new ValidationRuleRegistry(query).CreateAccessors().ToDictionary(x => (MessageTypeCode)x.MessageTypeId, x => x);
            _equalityComparer = equalityComparerFactory.CreateCompleteComparer<Version.ValidationResult>();
            _cache = cache;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // Общая идея: "результаты проверок соответствуют указанному состоянию ERM или более позднему"
            // Конкретные идеи:
            //  1. Набор ValidationResult для версии не меняется, таблица ValidationResult работает только на наполнение (за исключениям операции архивирования)
            //  2. Набор ErmStates для версии мутабелен, при этом может быть пустым (если мы уже обработали изменения, но пока не знаем, какой версии они соответствуют)
            //  3. Версия без изменений не создаётся.

            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).Take(1).AsEnumerable().First().Id;
            var newValidationResults = new List<Version.ValidationResult>();
            var resolvedValidationResults = new List<Version.ValidationResult>();

            var ruleGroups = commands.OfType<IRecalculateValidationRuleCommand>().GroupBy(x => x.Rule).ToList();
            if (ruleGroups.Count != 0)
            {
                foreach (var ruleCommands in ruleGroups)
                {
                    using (Probe.Create($"Rule {ruleCommands.Key}"))
                    {
                        var targetValidationResults = QueryTarget(ruleCommands.Key, currentVersion);

                        var filter = CreateFilter(ruleCommands);
                        var validationRuleResult = CalculateValidationRuleChanges(targetValidationResults, ruleCommands.Key, filter);

                        var newResults = validationRuleResult.Difference.ToList();
                        var resolvedResults = validationRuleResult.Complement.ToList();

                        newValidationResults.AddRange(newResults);
                        resolvedValidationResults.AddRange(resolvedResults);

                        // validationRuleResult.Intersection не используется, т.к. он содержит только те записи, что прошли через filter
                        UpdateCache(ruleCommands.Key, targetValidationResults, newResults, resolvedResults);
                    }
                }
            }

            var ermStates = commands.OfType<StoreErmStateCommand>().SelectMany(x => x.States).ToList();
            var amsStates = commands.OfType<StoreAmsStateCommand>().Select(x => x.State).ToList();
            if (newValidationResults.Count > 0 || resolvedValidationResults.Count > 0)
            {
                using (Probe.Create("Create New Version"))
                {
                    CreateVersion(currentVersion + 1, newValidationResults.Concat(resolvedValidationResults.ApplyResolved()).ToList(), ermStates, amsStates);
                }
            }
            else
            {
                using (Probe.Create("Update Existing Version"))
                {
                    UpdateVersion(currentVersion, ermStates, amsStates);
                }
            }

            return Array.Empty<IEvent>();
        }

        private IReadOnlyCollection<Version.ValidationResult> QueryTarget(MessageTypeCode messageType, long version)
        {
            using (Probe.Create("Query Cache"))
            {
                if (_cache.TryGet(messageType, out var targetValidationResults))
                {
                    return targetValidationResults;
                }
            }

            using (Probe.Create("Query Target"))
            {
                var targetValidationResults =
                    _query
                        .For<Version.ValidationResult>()
                        .Where(x => x.MessageType == (int)messageType)
                        .ForVersion(version)
                        .ApplyVersionId(0)
                        .ToList();

                _cache.Initialize(messageType, targetValidationResults);

                return targetValidationResults;
            }
        }

        private void UpdateCache(MessageTypeCode messageType, IReadOnlyCollection<Version.ValidationResult> existing, IReadOnlyCollection<Version.ValidationResult> newResults, IReadOnlyCollection<Version.ValidationResult> resolvedResults)
        {
            if (newResults.Count == 0 && resolvedResults.Count == 0)
            {
                return;
            }

            using (Probe.Create("Update Cache"))
            {
                var hs = new HashSet<Version.ValidationResult>(existing, _equalityComparer);

                hs.UnionWith(newResults);
                hs.ExceptWith(resolvedResults);

                _cache.Update(messageType, hs);
            }
        }

        private static Expression<Func<Version.ValidationResult, bool>> CreateFilter(IEnumerable<IRecalculateValidationRuleCommand> commands)
        {
            var ids = new HashSet<long>();
            foreach (var command in commands)
            {
                switch (command)
                {
                    case RecalculateValidationRuleCompleteCommand _:
                        return x => true;

                    case RecalculateValidationRulePartiallyCommand recalculateRulePartiallyCommand:
                        ids.UnionWith(recalculateRulePartiallyCommand.Filter);
                        break;
                }
            }

            return x => x.OrderId.HasValue && ids.Contains(x.OrderId.Value);
        }

        private MergeResult<Version.ValidationResult> CalculateValidationRuleChanges(IReadOnlyCollection<Version.ValidationResult> currentVersionResults, MessageTypeCode ruleCode, Expression<Func<Version.ValidationResult, bool>> filter)
        {
            try
            {
                List<Version.ValidationResult> sourceObjects;

                using (Probe.Create("Query Source"))
                {
                    var accessor = _accessors[ruleCode];
                    var query = accessor.GetSource().Where(filter);
                    sourceObjects = query.ToList();
                }

                using (Probe.Create("Merge"))
                {
                    var destObjects = currentVersionResults.Where(x => x.MessageType == (int)ruleCode).Where(filter.Compile());
                    var mergeResult = MergeTool.Merge(sourceObjects, destObjects, _equalityComparer);
                    return mergeResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при вычислении правила {ruleCode}", ex);
            }
        }

        private void CreateVersion(long id, IReadOnlyCollection<Version.ValidationResult> results, IReadOnlyCollection<ErmState> ermStates, IReadOnlyCollection<AmsState> amsStates)
        {
            _versionRepository.Add(new Version { Id = id, UtcDateTime = DateTime.UtcNow });
            _versionRepository.Save();

            if (results.Count != 0)
            {
                _validationResultRepository.Create(results.ApplyVersionId(id));
            }

            if (ermStates.Count != 0)
            {
                var versionErmStates = ermStates.Select(x => new Version.ErmState {VersionId = id, Token = x.Token, UtcDateTime = x.UtcDateTime});
                _syncInMemoryErmStates.ExecuteCommands(new[] {new SyncInMemoryDataObjectCommand(typeof(Version.ErmState), versionErmStates)});
            }

            if (amsStates.Count != 0)
            {
                var maxAmsState = amsStates.Aggregate((a, b) => a.Offset > b.Offset ? a : b);
                _amsStatesRepository.Add(new Version.AmsState { VersionId = id, Offset = maxAmsState.Offset, UtcDateTime = maxAmsState.UtcDateTime });
                _amsStatesRepository.Save();
            }
        }

        private void UpdateVersion(long id, IReadOnlyCollection<ErmState> ermStates, IReadOnlyCollection<AmsState> amsStates)
        {
            _versionRepository.Update(new Version { Id = id, UtcDateTime = DateTime.UtcNow });
            _versionRepository.Save();

            if (ermStates.Count != 0)
            {
                var versionErmStates = ermStates.Select(x => new Version.ErmState {VersionId = id, Token = x.Token, UtcDateTime = x.UtcDateTime});
                _syncInMemoryErmStates.ExecuteCommands(new[] {new SyncInMemoryDataObjectCommand(typeof(Version.ErmState), versionErmStates)});
            }

            if (amsStates.Count != 0)
            {
                var maxAmsState = amsStates.Aggregate((a, b) => a.Offset > b.Offset ? a : b);
                _amsStatesRepository.Add(new Version.AmsState { VersionId = id, Offset = maxAmsState.Offset, UtcDateTime = maxAmsState.UtcDateTime });
                _amsStatesRepository.Save();
            }
        }

        private sealed class ValidationRuleRegistry
        {
            private readonly IQuery _query;

            public ValidationRuleRegistry(IQuery query)
            {
                _query = query;
            }

            public IEnumerable<IValidationResultAccessor> CreateAccessors()
            {
                return new IValidationResultAccessor[]
                {
                    new BargainScanShouldPresent(_query),
                    new LegalPersonProfileBargainShouldNotBeExpired(_query),
                    new LegalPersonProfileWarrantyShouldNotBeExpired(_query),
                    new LegalPersonShouldHaveAtLeastOneProfile(_query),
                    new LinkedCategoryAsteriskMayBelongToFirm(_query),
                    new LinkedCategoryFirmAddressShouldBeValid(_query),
                    new LinkedCategoryShouldBeActive(_query),
                    new LinkedCategoryShouldBelongToFirm(_query),
                    new LinkedFirmAddressShouldBeValid(_query),
                    new LinkedFirmShouldBeValid(_query),
                    new BillsSumShouldMatchOrder(_query),
                    new BillsShouldBeCreated(_query),
                    new OrderMustHaveActiveDeal(_query),
                    new OrderMustHaveActiveLegalEntities(_query),
                    new OrderRequiredFieldsShouldBeSpecified(_query),
                    new OrderScanShouldPresent(_query),
                    new OrderShouldHaveAtLeastOnePosition(_query),
                    new OrderShouldNotBeSignedBeforeBargain(_query),

                    new AccountShouldExist(_query),
                    new AccountBalanceShouldBePositive(_query),
                    new AtLeastOneLinkedPartnerFirmAddressShouldBeValid(_query),

                    // AdvertisementRules
                    new OrderPositionAdvertisementMustBeCreated(_query),
                    new OrderPositionAdvertisementMustHaveAdvertisement(_query),
                    new OrderPositionAdvertisementMustHaveOptionalAdvertisement(_query),
                    new AdvertisementMustBelongToFirm(_query),
                    new AdvertisementMustPassReview(_query),
                    new OptionalAdvertisementMustPassReview(_query),
                    new AdvertisementShouldNotHaveComments(_query),
                    new AmsMessagesShouldBeNew(_query),

                    new AdvertisementCountPerCategoryShouldBeLimited(_query),
                    new AdvertisementCountPerThemeShouldBeLimited(_query),
                    new FirmAssociatedPositionMustHavePrincipal(_query),
                    new FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject(_query),
                    new FirmPositionMustNotHaveDeniedPositions(_query),
                    new FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject(_query),
                    new AdvertisementAmountShouldMeetMaximumRestrictions(_query),
                    new AdvertisementAmountShouldMeetMinimumRestrictions(_query),
                    new AdvertisementAmountShouldMeetMinimumRestrictionsMass(_query),
                    new OrderPositionCorrespontToInactivePosition(_query),
                    new OrderPositionMayCorrespontToActualPrice(_query),
                    new OrderPositionMustCorrespontToActualPrice(_query),
                    new OrderMustHaveActualPrice(_query),
                    new FirmAssociatedPositionShouldNotStayAlone(_query),

                    new FirmAndOrderShouldBelongTheSameOrganizationUnit(_query),
                    new FirmShouldHaveLimitedCategoryCount(_query),
                    new PartnerAdvertisementMustNotCauseProblemsToTheAdvertiser(_query),
                    new PartnerAdvertisementShouldNotHaveDifferentSalesModel(_query),

                    new FirmAddressMustBeLocatedOnTheMap(_query),
                    new OrderMustNotIncludeReleasedPeriod(_query),
                    new OrderMustUseCategoriesOnlyAvailableInProject(_query),
                    new OrderPositionCostPerClickMustBeSpecified(_query),
                    new OrderPositionCostPerClickMustNotBeLessMinimum(_query),
                    new OrderPositionSalesModelMustMatchCategorySalesModel(_query),
                    new ProjectMustContainCostPerClickMinimumRestriction(_query),

                    new PoiAmountForEntranceShouldMeetMaximumRestrictions(_query),

                    // ThemeRules
                    new DefaultThemeMustBeExactlyOne(_query),
                    new ThemeCategoryMustBeActiveAndNotDeleted(_query),
                    new ThemePeriodMustContainOrderPeriod(_query),
                    new DefaultThemeMustHaveOnlySelfAds(_query),
                };
            }
        }
    }
}

