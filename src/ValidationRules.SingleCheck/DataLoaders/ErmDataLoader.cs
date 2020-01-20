﻿using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.SingleCheck.Store;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.SingleCheck.DataLoaders
{
    public static partial class ErmDataLoader
    {
        public static void Load(long orderId, DataConnection query, IStore store, out ResolvedOrderSummary orderSummary)
        {
            var checkOrderIds = new[] { orderId };

            var order = query.GetTable<Order>()
                         .Where(x => checkOrderIds.Contains(x.Id))
                         .Execute()
                         .Single();
            store.Add(order);

            LoadReleaseWithdrawals(query, order, store);

            var bargainIds = new[] { order.BargainId };
            var dealIds = new[] { order.DealId };
            var firmIds = new[] { order.FirmId };
            var branchOfficeOrganizationUnitIds = new[] { order.BranchOfficeOrganizationUnitId };
            var legalPersonIds = new[] { order.LegalPersonId };

            var unlimitedOrder = query.GetTable<UnlimitedOrder>()
                                      .Where(x => checkOrderIds.Contains(x.OrderId))
                                      .Execute();
            store.AddRange(unlimitedOrder);

            var orderFiles = query.GetTable<OrderFile>()
                                  .Where(x => checkOrderIds.Contains(x.OrderId))
                                  .Execute();
            store.AddRange(orderFiles);

            var bills = query.GetTable<Bill>()
                             .Where(x => checkOrderIds.Contains(x.OrderId))
                             .Execute();
            store.AddRange(bills);

            var bargains = query.GetTable<Bargain>()
                                .Where(x => bargainIds.Contains(x.Id))
                                .Execute();
            store.AddRange(bargains);

            var bargainFiles = query.GetTable<BargainFile>()
                                    .Where(x => bargainIds.Contains(x.BargainId))
                                    .Execute();
            store.AddRange(bargainFiles);

            var deals = query.GetTable<Deal>()
                             .Where(x => dealIds.Contains(x.Id))
                             .Execute();
            store.AddRange(deals);

            //
            var branchOfficeOrganizationUnits = query.GetTable<BranchOfficeOrganizationUnit>()
                                                     .Where(x => branchOfficeOrganizationUnitIds.Contains(x.Id))
                                                     .Execute();
            store.AddRange(branchOfficeOrganizationUnits);
            var branchOfficeIds = branchOfficeOrganizationUnits.Select(x => x.BranchOfficeId).ToHashSet();

            var branchOffices = query.GetTable<BranchOffice>()
                                     .Where(x => branchOfficeIds.Contains(x.Id))
                                     .Execute();
            store.AddRange(branchOffices);

            var legalPersons = query.GetTable<LegalPerson>()
                                    .Where(x => legalPersonIds.Contains(x.Id))
                                    .Execute();
            store.AddRange(legalPersons);

            var legalPersonProfiles = query.GetTable<LegalPersonProfile>()
                                           .Where(x => legalPersonIds.Contains(x.LegalPersonId))
                                           .Execute();
            store.AddRange(legalPersonProfiles);

            //
            var orders = query.GetTable<Order>()
                              .Where(op => op.IsActive && !op.IsDeleted)
                              .Where(x => firmIds.Contains(x.FirmId))
                              .Where(x => new[] { 1, 2, 4, 5 }.Contains(x.WorkflowStepId))
                              .Where(x => x.AgileDistributionStartDate < order.AgileDistributionEndPlanDate && order.AgileDistributionStartDate < x.AgileDistributionEndPlanDate)
                              .Execute();
            store.AddRange(orders);
            var orderIds = orders.Select(x => x.Id).ToList();

            var orderPositions = query.GetTable<OrderPosition>()
                                      .Where(op => op.IsActive && !op.IsDeleted)
                                      .Where(op => orderIds.Contains(op.OrderId))
                                      .Execute();
            store.AddRange(orderPositions);
            var orderPositionIds = orderPositions.Select(x => x.Id).ToList();
            var usedPricePositionIds = orderPositions.Select(x => x.PricePositionId).ToHashSet();

            var opas = query.GetTable<OrderPositionAdvertisement>()
                            .Where(op => orderPositionIds.Contains(op.OrderPositionId))
                            .Execute();
            store.AddRange(opas);
            var themeIds = opas.Where(x => x.ThemeId.HasValue).Select(x => x.ThemeId.Value).ToList();
            var categoryIds = opas.Where(x => x.CategoryId.HasValue).Select(x => x.CategoryId.Value).ToList();
            var firmAddressIds = opas.Where(x => x.FirmAddressId.HasValue).Select(x => x.FirmAddressId.Value).ToList(); // список привязанных адресов из-за ЗМК может превышать список адресов фирмы

            var costs = query.GetTable<OrderPositionCostPerAny>()
                             .Where(x => orderPositionIds.Contains(x.OrderPositionId))
                             .Execute(); // Можно ужесточить: только проверямый заказ, только актуальные ставки
            store.AddRange(costs);

            //
            var themes = query.GetTable<Theme>()
                              .Where(x => themeIds.Contains(x.Id))
                              .Execute();
            store.AddRange(themes);

            var themeCategories = query.GetTable<ThemeCategory>()
                                       .Where(x => themeIds.Contains(x.ThemeId))
                                       .Where(x => categoryIds.Contains(x.CategoryId))
                                       .Execute();
            store.AddRange(themeCategories);

            var themeOrganizationUnits = query.GetTable<ThemeOrganizationUnit>()
                                              .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                                              .Where(x => themeIds.Contains(x.ThemeId))
                                              .Execute();
            store.AddRange(themeOrganizationUnits);

            var project = query.GetTable<Project>()
                               .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                               .Take(1)
                               .Execute()
                               .Single();
            store.Add(project);

            //
            var actualPrice = query.GetTable<Price>()
                                   .Where(x => !x.IsDeleted && x.IsPublished)
                                   .Where(x => x.ProjectId == project.Id && x.BeginDate <= order.AgileDistributionStartDate)
                                   .OrderByDescending(x => x.BeginDate)
                                   .Take(1)
                                   .Execute()
                                   .SingleOrDefault();
            if (actualPrice != null)
            {
                store.Add(actualPrice);
            }

            var usedPricePositions = query.GetTable<PricePosition>()
                                          .Where(x => usedPricePositionIds.Contains(x.Id))
                                          .Execute();
            store.AddRange(usedPricePositions);
            var monthlyUsedPrices = query.GetTable<Price>()
                                         .Where(x => x.ProjectId == project.Id && x.BeginDate >= order.AgileDistributionStartDate && x.BeginDate <= order.AgileDistributionEndPlanDate)
                                         .Execute();
            store.AddRange(monthlyUsedPrices);
            var usedPriceIds = usedPricePositions.Select(x => x.PriceId).Union(actualPrice != null ? new[] { actualPrice.Id } : Array.Empty<long>()).Union(monthlyUsedPrices.Select(x => x.Id)).ToList();

            var soldPackagesIds = usedPricePositions.Select(p => p.PositionId)
                                                    .ToHashSet();
            var soldPackageElementsIds = opas.Select(y => y.PositionId)
                                             .ToHashSet();

            var usedPositionIds = soldPackagesIds.Union(soldPackageElementsIds).ToList();

            var positions = query.GetTable<Position>()
                                 .Where(x => usedPositionIds.Contains(x.Id))
                                 .Execute(); // Можно ограничиться проверямым заказов
            store.AddRange(positions);

            var positionChilds = query.GetTable<PositionChild>()
                                      .Where(x => usedPositionIds.Contains(x.MasterPositionId) || usedPositionIds.Contains(x.ChildPositionId))
                                      .Execute();
            store.AddRange(positionChilds);

            // Нужно ли ещё от PositionChild выбрать Position?

            var usedPrices = query.GetTable<Price>()
                                  .Where(x => usedPriceIds.Contains(x.Id))
                                  .Execute();
            store.AddRange(usedPrices);

            var releaseInfos = query.GetTable<ReleaseInfo>()
                                    .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                                    .Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == 2)
                                    .Execute(); // можно только последний?
            store.AddRange(releaseInfos);

            var categories = query.GetTable<Category>()
                                  .Where(x => categoryIds.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories);
            var cat2Ids = categories.Select(x => x.ParentId);

            var categories2 = query.GetTable<Category>()
                                  .Where(x => cat2Ids.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories2);
            var cat1Ids = categories2.Select(x => x.ParentId);

            var categories1 = query.GetTable<Category>()
                                  .Where(x => cat1Ids.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories1);

            var categoryOrganizationUnit =
                query.GetTable<CategoryOrganizationUnit>()
                     .Where(x => cat1Ids.Union(cat2Ids).Union(categoryIds).Contains(x.CategoryId))
                     .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                     .Execute();
            store.AddRange(categoryOrganizationUnit);

            var costPerClickCategoryRestrictions = query.GetTable<CostPerClickCategoryRestriction>()
                                                        .Where(x => x.ProjectId == project.Id)
                                                        .Where(x => categoryIds.Contains(x.CategoryId))
                                                        .Execute(); // Можно ужесточить: рубрики из свзанных заказов нам на самом деле не нужны.
            store.AddRange(costPerClickCategoryRestrictions);

            if(costPerClickCategoryRestrictions.Any())
            {
                var maxDate = costPerClickCategoryRestrictions.Max(x => x.BeginningDate);
                var nextCostPerClickCategoryRestrictions = query.GetTable<CostPerClickCategoryRestriction>()
                                                            .Where(x => x.ProjectId == project.Id)
                                                            .Where(x => x.BeginningDate > maxDate)
                                                            .Take(1)
                                                            .Execute(); // Нужно для того, чтобы понять, что имеющиеся ограчения не являются актуальными
                store.AddRange(nextCostPerClickCategoryRestrictions);
            }

            var salesModelCategoryRestrictions = query.GetTable<SalesModelCategoryRestriction>()
                                                      .Where(x => x.ProjectId == project.Id)
                                                      .Where(x => categoryIds.Contains(x.CategoryId))
                                                      .Execute(); // Можно ужесточить: рубрики из свзанных заказов нам на самом деле не нужны.
            store.AddRange(salesModelCategoryRestrictions);

            var nomenclatureCategories = query.GetTable<NomenclatureCategory>()
                                              .Execute();
            store.AddRange(nomenclatureCategories);

            LoadAmountControlledSales(query, order, usedPriceIds, store);
            LoadFirm(query, order, firmAddressIds, store);
            LoadBuyHere(query, order, store);
            LoadPoi(query, order, store);

            orderSummary = new ResolvedOrderSummary
            {
                BeginDate = order.AgileDistributionStartDate,
                EndDate = order.AgileDistributionEndPlanDate,

                ProjectId = project.Id,

                SoldPackagesIds = soldPackagesIds,
                SoldPackageElementsIds = soldPackageElementsIds
            };
        }

        private static void LoadFirm(DataConnection query, Order order, IReadOnlyCollection<long> additionalFirmIds, IStore store)
        {
            var firms =
                query.GetTable<Firm>()
                     .Where(x => x.Id == order.FirmId)
                     .Execute();
            store.AddRange(firms);
            var firmIds = firms.Select(x => x.Id);

            var firmAddresses =
                query.GetTable<FirmAddress>().Where(x => firmIds.Contains(x.FirmId))
                    .Union(query.GetTable<FirmAddress>().Where(x => additionalFirmIds.Contains(x.Id)))
                    .Execute();
            store.AddRange(firmAddresses);
            var firmAddressIds = firmAddresses.Select(y => y.Id).ToList();

            var categoryFirmAddresses =
                query.GetTable<CategoryFirmAddress>()
                     .Where(x => firmAddressIds.Contains(x.FirmAddressId))
                     .Execute();
            store.AddRange(categoryFirmAddresses);
        }

        private static void LoadReleaseWithdrawals(DataConnection query, Order order, IStore store)
        {
            var orders =
                query.GetTable<Order>()
                     .Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                     .Where(x => x.AgileDistributionStartDate < order.AgileDistributionEndPlanDate && order.AgileDistributionStartDate < x.AgileDistributionEndFactDate)
                     .Execute();
            var orderIds = orders.Select(x => x.Id);
            store.AddRange(orders);

            var orderPositions =
                query.GetTable<OrderPosition>()
                     .Where(x => orderIds.Contains(x.OrderId))
                     .Execute();
            var orderPositionIds = orderPositions.Select(x => x.Id);
            store.AddRange(orderPositions);

            var releaseWithdrawals =
                query.GetTable<ReleaseWithdrawal>()
                     .Where(x => orderPositionIds.Contains(x.OrderPositionId))
                     .Execute();
            store.AddRange(releaseWithdrawals);
        }

        private static void LoadAmountControlledSales(DataConnection query, Order order, IReadOnlyCollection<long> priceIds, IStore store)
        {
            var categoryCodes =
                (from orderPosition in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == order.Id)
                 from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                 from position in query.GetTable<Position>().Where(x => (x.IsControlledByAmount || x.CategoryCode == Storage.Model.Facts.Position.CategoryCodeAdvertisementInCategory) && x.Id == opa.PositionId)
                 select position.CategoryCode).Distinct().Execute();

            var orders =
                 from interferringOrder in query.GetTable<Order>().Where(x => x.IsActive && !x.IsDeleted && new[] { 1, 2, 4, 5 }.Contains(x.WorkflowStepId))
                                                .Where(x => x.DestOrganizationUnitId == order.DestOrganizationUnitId && x.AgileDistributionStartDate < order.AgileDistributionEndPlanDate && order.AgileDistributionStartDate < x.AgileDistributionEndPlanDate)
                 from orderPosition in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == interferringOrder.Id)
                 from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                 from position in query.GetTable<Position>().Where(x => (x.IsControlledByAmount || x.CategoryCode == Storage.Model.Facts.Position.CategoryCodeAdvertisementInCategory) && x.Id == opa.PositionId && categoryCodes.Contains(x.CategoryCode))
                 from pricePosition in query.GetTable<PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                 select new { interferringOrder, orderPosition, opa, position, pricePosition };

            store.AddRange(orders.Select(x => x.interferringOrder).Execute());
            store.AddRange(orders.Select(x => x.orderPosition).Execute());
            store.AddRange(orders.Select(x => x.opa).Execute());
            store.AddRange(orders.Select(x => x.position).Execute());
            store.AddRange(orders.Select(x => x.pricePosition).Execute()); // Нужны для PriceCOntext.Order.OrderPostion
        }

        public sealed class ResolvedOrderSummary
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }

            public long ProjectId { get; set; }

            public IReadOnlyCollection<long> SoldPackagesIds { get; set; }
            public IReadOnlyCollection<long> SoldPackageElementsIds { get; set; }
        }
    }
}

