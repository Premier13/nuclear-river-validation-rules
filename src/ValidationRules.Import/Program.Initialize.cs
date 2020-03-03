using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using NuClear.ValidationRules.Import.Model.PersistentFacts;
using NuClear.ValidationRules.Import.Model.Service;
using NuClear.ValidationRules.Import.Processing;

namespace NuClear.ValidationRules.Import
{
    public static partial class Program
    {
        private static readonly TransactionOptions TransactionOptions
            = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted};

        private static Command CreateInitializeCommand()
        {
            var command = new Command("initialize", "Prepare DB structure")
            {
                new Option("--database", "Database connection string") {Argument = new Argument<string>()},
            };
            command.Handler =
                CommandHandler.Create<string>(Initialize);

            return command;
        }

        private static void Initialize(string database)
        {
            try
            {
                var dataConnectionFactory = new DataConnectionFactory(database, Configurations);
                using var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionOptions);
                using var dataConnection = dataConnectionFactory.Create();

                // todo: сделать полноценную инициализацию БД как в "большом" VR
                var schemas = new[] {"PersistentFacts", "Service"};
                foreach (var schema in schemas)
                {
                    dataConnection.Execute(
                        $"if not exists (select * from sys.schemas where name = '{schema}') exec('create schema {schema}')");
                }

                // todo: если в кафке не полный снимок данных - удаление таблиц недопустимо, нужны миграции.
                dataConnection.DropTable<Account>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<AccountDetail>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<BranchOffice>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<BranchOfficeOrganizationUnit>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<CostPerClickCategoryRestriction>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<LegalPerson>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<LegalPersonProfile>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<NomenclatureCategory>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<Position>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<PositionChild>(throwExceptionIfNotExists: false);
                dataConnection.DropTable<SalesModelCategoryRestriction>(throwExceptionIfNotExists: false);

                dataConnection.CreateTable<Account>();
                dataConnection.CreateTable<AccountDetail>();
                dataConnection.CreateTable<BranchOffice>();
                dataConnection.CreateTable<BranchOfficeOrganizationUnit>();
                dataConnection.CreateTable<CostPerClickCategoryRestriction>();
                dataConnection.CreateTable<LegalPerson>();
                dataConnection.CreateTable<LegalPersonProfile>();
                dataConnection.CreateTable<NomenclatureCategory>();
                dataConnection.CreateTable<Position>();
                dataConnection.CreateTable<PositionChild>();
                dataConnection.CreateTable<SalesModelCategoryRestriction>();

                // ??

                dataConnection.DropTable<EventRecord>(throwExceptionIfNotExists: false);
                dataConnection.CreateTable<EventRecord>();

                dataConnection.DropTable<ConsumerState>(throwExceptionIfNotExists: false);
                dataConnection.CreateTable<ConsumerState>();

                dataConnection.DropTable<EntityName>(throwExceptionIfNotExists: false);
                dataConnection.CreateTable<EntityName>();

                // todo: создание индексов, как в большом приложении.

                transaction.Complete();
            }
            catch (Exception e)
            {
                Log.Error("Fatal error", e);
                throw;
            }
        }
    }
}
