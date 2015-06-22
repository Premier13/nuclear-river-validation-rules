﻿using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public sealed class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IQuery _query;

        public CustomerIntelligenceTransformationContext(IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _query = query;
        }

        public IQueryable<Category> Categories
        {
            get
            {
                return from category in _query.For<Facts.Category>()
                       select new Category
                       {
                           Id = category.Id,
                           Name = category.Name,
                           Level = category.Level,
                           ParentId = category.ParentId
                       };
            }
        }

        public IQueryable<CategoryGroup> CategoryGroups
        {
            get
            {
                return from categoryGroup in _query.For<Facts.CategoryGroup>()
                       select new CategoryGroup
                       {
                           Id = categoryGroup.Id,
                           Name = categoryGroup.Name,
                           Rate = categoryGroup.Rate
                       };
            }
        }

        public IQueryable<Client> Clients
        {
            get
            {
                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from client in _query.For<Facts.Client>()
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  //CategoryGroupId = null
                              };
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _query.For<Facts.Contact>()
                       select new Contact
                              {
                                  Id = contact.Id,
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                                  ClientId = contact.ClientId
                              };
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                // FIXME {all, 03.04.2015}: the obtained SQL is too complex and slow

                var clientsHavingPhone = from contact in _query.For<Facts.Contact>()
                                         where contact.HasPhone
                                         select contact.ClientId;
                var clientsHavingWebsite = from contact in _query.For<Facts.Contact>()
                                           where contact.HasWebsite
                                           select contact.ClientId;

                var firmsHavingPhone = from firmContact in _query.For<Facts.FirmContact>().Where(x => x.HasPhone)
                                       join firmAddress in _query.For<Facts.FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                       select firmAddress.FirmId;
                var firmsHavingWebsite = from firmContact in _query.For<Facts.FirmContact>().Where(x => x.HasWebsite)
                                         join firmAddress in _query.For<Facts.FirmAddress>() on firmContact.FirmAddressId equals firmAddress.Id
                                         select firmAddress.FirmId;

                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from firm in _query.For<Facts.Firm>()
                       join project in _query.For<Facts.Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                       join client in _query.For<Facts.Client>() on firm.ClientId equals client.Id into firmClients
                       from firmClient in firmClients.DefaultIfEmpty()
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = _query.For<Facts.Order>().Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                  AddressCount = _query.For<Facts.FirmAddress>().Count(fa => fa.FirmId == firm.Id),
                                  //CategoryGroupId = null,
                                  ClientId = firm.ClientId,
                                  ProjectId = project.Id,
                                  OwnerId = firm.OwnerId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

        public IQueryable<FirmBalance> FirmBalances
        {
            get
            {
                return from account in _query.For<Facts.Account>()
                       join legalPerson in _query.For<Facts.LegalPerson>() on account.LegalPersonId equals legalPerson.Id
                       join client in _query.For<Facts.Client>() on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in _query.For<Facts.BranchOfficeOrganizationUnit>() on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in _query.For<Facts.Firm>() on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance };
            }
        }

        public IQueryable<FirmCategory> FirmCategories
        {
            get
            {
                var categories1 = _query.For<Facts.Category>().Where(x => x.Level == 1);
                var categories2 = _query.For<Facts.Category>().Where(x => x.Level == 2);
                var categories3 = _query.For<Facts.Category>().Where(x => x.Level == 3);

                var level3 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category3.Id };

                var level2 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category2.Id };

                var level1 = from firmAddress in _query.For<Facts.FirmAddress>()
                             join categoryFirmAddress in _query.For<Facts.CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             join category1 in categories1 on category2.ParentId equals category1.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category1.Id };

                // perform union using distinct
                return level3.Union(level2).Union(level1);
            }
        }

        public IQueryable<Project> Projects
        {
            get
            {
                return from project in _query.For<Facts.Project>()
                       select new Project
                       {
                           Id = project.Id,
                           Name = project.Name
                       };
            }
        }

        public IQueryable<ProjectCategory> ProjectCategories
        {
            get
            {
                return from project in _query.For<Facts.Project>()
                       join categoryOrganizationUnit in _query.For<Facts.CategoryOrganizationUnit>() on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                       select new ProjectCategory
                       {
                           ProjectId = project.Id,
                           CategoryId = categoryOrganizationUnit.CategoryId,
                       };
            }
        }

        public IQueryable<Territory> Territories
        {
            get
            {
                return from territory in _query.For<Facts.Territory>()
                       join project in _query.For<Facts.Project>() on territory.OrganizationUnitId equals project.OrganizationUnitId
                       select new Territory
                       {
                           Id = territory.Id,
                           Name = territory.Name,
                           ProjectId = project.Id
                       };
            }
        }
    }
}