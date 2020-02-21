using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules;
using NuClear.ValidationRules.Storage.Model.Messages;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PartnerAdvertisementShouldNotHaveDifferentSalesModel
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PartnerAdvertisementShouldNotHaveDifferentSalesModel))
                .Aggregate(
                    // Одобренный заказ с змк в адрес третьей фирмы (не видит второго)
                    new Order { Id = 1, Start = MonthStart(1), End = MonthStart(3), FirmId = 1, Scope = 0 },
                    new Order.PartnerPosition { OrderId = 1, OrderPositionId = 1, DestinationFirmId = 3, DestinationFirmAddressId = 3, SalesModel = 10 },

                    // Заказ на оформлении с змк в адрес третьей фирмы (видит первого)
                    new Order { Id = 2, Start = MonthStart(2), End = MonthStart(4), FirmId = 2, Scope = 2 },
                    new Order.PartnerPosition { OrderId = 2, OrderPositionId = 2, DestinationFirmId = 3, DestinationFirmAddressId = 3, SalesModel = 12 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(2),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeFirmAddress>(3))
                                .ToXDocument(),
                        MessageType = (int)MessageTypeCode.PartnerAdvertisementShouldNotHaveDifferentSalesModel,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(3),
                        OrderId = 2,
                    });
    }
}

