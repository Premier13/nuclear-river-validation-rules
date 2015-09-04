using System;
using System.Data.Common;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.AdvancedSearch.Replication.Tests.Data;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.BulkLoading
{
    [TestFixture, Explicit("It's used to copy the data in bulk.")]
    internal class BulkLoadingOfFacts : BulkLoadingFixtureBase
    {
        [Test]
        public void ReloadAccounts()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Accounts.Map(query));
        }

        [Test]
        public void ReloadActivities()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Activities.Map(query));
        }

        [Test]
        public void ReloadBranchOfficeOrganizationUnits()
        {
            Reload(query => Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits.Map(query));
        }

        [Test]
        public void ReloadCategories()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Categories.Map(query));
        }

        [Test]
        public void ReloadCategoryGroups()
        {
            Reload(query => Specs.Map.Erm.ToFacts.CategoryGroups.Map(query));
        }

        [Test]
        public void ReloadCategoryFirmAddresses()
        {
            Reload(query => Specs.Map.Erm.ToFacts.CategoryFirmAddresses.Map(query));
        }

        [Test]
        public void ReloadCategoryOrganizationUnits()
        {
            Reload(query => Specs.Map.Erm.ToFacts.CategoryOrganizationUnits.Map(query));
        }

        [Test]
        public void ReloadClients()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Clients.Map(query));
        }

        [Test]
        public void ReloadContacts()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Contacts.Map(query));
        }

        [Test]
        public void ReloadFirms()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Firms.Map(query));
        }

        [Test]
        public void ReloadFirmAddresses()
        {
            Reload(query => Specs.Map.Erm.ToFacts.FirmAddresses.Map(query));
        }

        [Test]
        public void ReloadFirmContacts()
        {
            Reload(query => Specs.Map.Erm.ToFacts.FirmContacts.Map(query));
        }

        [Test]
        public void ReloadLegalPersons()
        {
            Reload(query => Specs.Map.Erm.ToFacts.LegalPersons.Map(query));
        }

        [Test]
        public void ReloadOrders()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Orders.Map(query));
        }

        [Test]
        public void ReloadProjects()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Projects.Map(query));
        }

        [Test]
        public void ReloadTerritories()
        {
            Reload(query => Specs.Map.Erm.ToFacts.Territories.Map(query));
        }

        private void Reload<T>(Func<IQuery, IQueryable<T>> loader)
            where T : class
        {
            using (var ermDb = CreateConnection("ErmSqlServer", Schema.Erm))
            using (var factDb = CreateConnection("FactsSqlServer", Schema.Facts))
            {
                var query = new Query(new StubReadableDomainContextProvider((DbConnection)ermDb.Connection, ermDb));
                factDb.Reload(loader(query));
            }
        }
    }
}