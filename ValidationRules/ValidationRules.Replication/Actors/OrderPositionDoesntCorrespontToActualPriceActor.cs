using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;

using NuClear.ValidationRules.Storage.Model.Aggregates;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class OrderPositionDoesntCorrespontToActualPriceActor : IActor
    {
        // OrderCheckOrderPositionDoesntCorrespontToActualPrice - ������� {OrderPositionId} �� ������������� ����������� �����-�����. ���������� ������� ������� �� �������� ������������ �����-�����.
        private const int MessageTypeId = 5;

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;

        public OrderPositionDoesntCorrespontToActualPriceActor(IQuery query, IBulkRepository<Version.ValidationResult> repository)
        {
            _query = query;
            _repository = repository;
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
            var targetObjects = _query.For<Version.ValidationResult>().Where(x => x.MessageType == MessageTypeId && x.VersionId == 0).ToArray();
            _repository.Delete(targetObjects);
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
                                  on new PeriodKey { OrganizationUnitId = orderFirstPeriod.OrganizationUnitId, Start = orderFirstPeriod.Start }
                                  equals new PeriodKey { OrganizationUnitId = period.OrganizationUnitId, Start = period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,

                                      period.OrganizationUnitId,
                                      period.Start,
                                      period.End,
                                  };

            var orderPositionBadPriceErrors =
            from orderFirstPeriodDto in orderFirstPeriodDtos
            from pricePeriod in query.For<PricePeriod>().Where(x => x.OrganizationUnitId == orderFirstPeriodDto.OrganizationUnitId && x.Start == orderFirstPeriodDto.Start).DefaultIfEmpty()
            join orderPricePosition in query.For<OrderPricePosition>() on orderFirstPeriodDto.OrderId equals orderPricePosition.OrderId
            where pricePeriod != null
            where orderPricePosition.PriceId != null
            where pricePeriod.PriceId != orderPricePosition.PriceId.Value
            select new Version.ValidationResult
            {
                MessageType = MessageTypeId,
                MessageParams = new XDocument(new XElement("empty", new XAttribute("name", orderPricePosition.PositionName))),
                OrderId = orderFirstPeriodDto.OrderId,
                PeriodStart = orderFirstPeriodDto.Start,
                PeriodEnd = orderFirstPeriodDto.End,
                OrganizationUnitId = orderFirstPeriodDto.OrganizationUnitId,
                Result = 1,
                VersionId = version
            };

            return orderPositionBadPriceErrors;
        }
    }
}