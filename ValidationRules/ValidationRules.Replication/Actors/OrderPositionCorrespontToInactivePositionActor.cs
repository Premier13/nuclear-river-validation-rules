using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;

using NuClear.ValidationRules.Storage.Model.Aggregates;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class OrderPositionCorrespontToInactivePositionActor : IActor
    {
        // OrderCheckOrderPositionCorrespontToInactivePosition - ������� {OrderPositionId} ������������� ������� ������� ����� �����. ���������� ������� �������� ������� �� �������� ������������ �����-�����.
        private const int MessageTypeId = 4;

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;

        public OrderPositionCorrespontToInactivePositionActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository)
        {
            _query = query;
            _repository = repository;
            _deleteRepository = deleteRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // todo: �������� � ������������ � ��������� ����� ������
            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;

            IReadOnlyCollection<Version.ValidationResult> sourceObjects;
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                // ������ � ������ ��������� �������� ��� ����������, ������� ���� �� ����� ���� �� ������.
                sourceObjects = GetValidationResults(_query, currentVersion).ToArray();

                // todo: �������, ��������� � ����� �������
                sourceObjects = sourceObjects.Where(x => x.PeriodStart >= DateTime.Parse("2016-06-01")).ToArray();

                scope.Complete();
            }

            // ������ � ������� �������� ������ � ����� ������� ���������� (������ ��� ����������� �� ��������)
            var forBulkDelete = new Version.ValidationResultForBulkDelete { MessageType = MessageTypeId, VersionId = currentVersion };
            _deleteRepository.Delete(new[] { forBulkDelete });
            _repository.Create(sourceObjects);

            return Array.Empty<IEvent>();
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            // �������� ��������� ������������ ������ ������� �������
            var orderFirstPeriods = from orderPeriod1 in query.For<OrderPeriod>()
                                    from orderPeriod2 in query.For<OrderPeriod>().Where(x => orderPeriod1.OrderId == x.OrderId && orderPeriod1.Start > x.Start).DefaultIfEmpty()
                                    where orderPeriod2 == null
                                    select orderPeriod1;

            var orderFirstPeriodDtos = from orderFirstPeriod in orderFirstPeriods
                                  join order in query.For<Order>() on orderFirstPeriod.OrderId equals order.Id
                                  join period in query.For<Period>()
                                  on new PeriodKey { ProjectId = orderFirstPeriod.ProjectId, Start = orderFirstPeriod.Start }
                                  equals new PeriodKey { ProjectId = period.ProjectId, Start = period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,

                                      period.ProjectId,
                                      period.Start,
                                      period.End,
                                  };

            var pricePositionIsNotActiveErrors =
            from orderFirstPeriodDto in orderFirstPeriodDtos
            join orderPricePosition in query.For<OrderPricePosition>() on orderFirstPeriodDto.OrderId equals orderPricePosition.OrderId
            where orderPricePosition.PriceId == null
            select new Version.ValidationResult
            {
                MessageType = MessageTypeId,
                // TODO: �������� - �� ���� �������� �������� ���������� �������, ��� ��� �� �� ��� ������������� �� ����� facts
                MessageParams = null,

                OrderId = orderFirstPeriodDto.OrderId,
                PeriodStart = orderFirstPeriodDto.Start,
                PeriodEnd = orderFirstPeriodDto.End,
                ProjectId = orderFirstPeriodDto.ProjectId,

                Result = 1,
                VersionId = version
            };

            return pricePositionIsNotActiveErrors;
        }
    }
}