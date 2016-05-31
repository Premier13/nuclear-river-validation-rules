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
    public class RefereceCurrentPriceListActor : IActor
    {
        // OrderCheckOrderPositionsDoesntCorrespontToActualPrice - ������� �� ������������� ����������� �����-�����. ���������� ������� ������� �� �������� ������������ �����-�����.
        private const int PriceNotFound = 3;

        // OrderCheckOrderPositionCorrespontToInactivePosition - ������� {OrderPositionId} ������������� ������� ������� ����� �����. ���������� ������� �������� ������� �� �������� ������������ �����-�����.
        private const int PricePositionIsNotActive = 4;

        // OrderCheckOrderPositionDoesntCorrespontToActualPrice - ������� {OrderPositionId} �� ������������� ����������� �����-�����. ���������� ������� ������� �� �������� ������������ �����-�����.
        private const int OrderPositionBadPrice = 5;

        private readonly int[] _messageTypes =
        {
            PriceNotFound,
            PricePositionIsNotActive,
            OrderPositionBadPrice
        };

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;

        public RefereceCurrentPriceListActor(IQuery query, IBulkRepository<Version.ValidationResult> repository)
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
            var targetObjects = _query.For<Version.ValidationResult>().Where(x => _messageTypes.Contains(x.MessageType) && x.VersionId == 0).ToArray();
            _repository.Delete(targetObjects);
            _repository.Create(sourceObjects);

            return Array.Empty<IEvent>();
        }

        private static IEnumerable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            // TODO: ����������� �������� �� � ���� ������
            var validationResults = new List<Version.ValidationResult>();

            var orderPeriodDtos = from order in query.For<Order>()
                                  join orderPeriod in query.For<OrderPeriod>() on order.Id equals orderPeriod.OrderId
                                  join period in query.For<Period>()
                                  on new PeriodKey { OrganizationUnitId = orderPeriod.OrganizationUnitId, Start = orderPeriod.Start }
                                  equals new PeriodKey { OrganizationUnitId = period.OrganizationUnitId, Start = period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,

                                      period.OrganizationUnitId,
                                      period.Start,
                                      period.End,
                                  };


            var priceNotFoundErrors =
            from orderPeriodDto in orderPeriodDtos
            from pricePeriod in query.For<PricePeriod>().Where(x => x.OrganizationUnitId == orderPeriodDto.OrganizationUnitId && x.Start == orderPeriodDto.Start).DefaultIfEmpty()
            where pricePeriod == null
            select new Version.ValidationResult
            {
                MessageType = PriceNotFound,
                MessageParams = null,
                OrderId = orderPeriodDto.OrderId,
                PeriodStart = orderPeriodDto.Start,
                PeriodEnd = orderPeriodDto.End,
                OrganizationUnitId = orderPeriodDto.OrganizationUnitId,
                Result = 1,
                VersionId = version,
            };

            validationResults.AddRange(priceNotFoundErrors);

            var pricePositionIsNotActiveErrors =
            from orderPeriodDto in orderPeriodDtos
            join orderPricePosition in query.For<OrderPricePosition>() on orderPeriodDto.OrderId equals orderPricePosition.OrderId
            where orderPricePosition.PriceId == null
            select new Version.ValidationResult
            {
                MessageType = PricePositionIsNotActive,
                // TODO: �������� - �� ���� �������� �������� ���������� �������, ��� ��� �� �� ��� ������������� �� ���������� �����
                MessageParams = null,

                OrderId = orderPeriodDto.OrderId,
                PeriodStart = orderPeriodDto.Start,
                PeriodEnd = orderPeriodDto.End,
                OrganizationUnitId = orderPeriodDto.OrganizationUnitId,

                Result = 1,
                VersionId = version
            };

            validationResults.AddRange(pricePositionIsNotActiveErrors);

            var orderPositionBadPriceErrors =
            from orderPeriodDto in orderPeriodDtos
            from pricePeriod in query.For<PricePeriod>().Where(x => x.OrganizationUnitId == orderPeriodDto.OrganizationUnitId && x.Start == orderPeriodDto.Start).DefaultIfEmpty()
            join orderPricePosition in query.For<OrderPricePosition>() on orderPeriodDto.OrderId equals orderPricePosition.OrderId
            where pricePeriod != null
            where orderPricePosition.PriceId != null
            where pricePeriod.PriceId != orderPricePosition.PriceId.Value
            select new Version.ValidationResult
            {
                MessageType = OrderPositionBadPrice,
                MessageParams = new XDocument(new XElement("empty", new XAttribute("name", orderPricePosition.PositionName))),
                OrderId = orderPeriodDto.OrderId,
                PeriodStart = orderPeriodDto.Start,
                PeriodEnd = orderPeriodDto.End,
                OrganizationUnitId = orderPeriodDto.OrganizationUnitId,
                Result = 1,
                VersionId = version
            };

            validationResults.AddRange(orderPositionBadPriceErrors);

            return validationResults;
        }
    }
}