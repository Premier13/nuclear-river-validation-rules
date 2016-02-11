﻿using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = Domain.Model.Facts;
    using Aggs = Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionTest
        => ArrangeMetadataElement.Config
        .Name(nameof(OrderPositionTest))
        .Fact(
            // OrderPositionAdvertisement
            new Facts::OrderPosition { Id = 1, PricePositionId = 1 },
            new Facts::PricePosition { Id = 1, PositionId = 2 },
            new Facts::OrderPositionAdvertisement {Id = 1, PositionId = 3, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 1 },

            // OrderPosition
            new Facts::OrderPosition { PricePositionId = 2 },
            new Facts::PricePosition { Id = 2, PositionId = 3 },
            new Facts::Position { Id = 3, IsComposite = true },

            // OrderPositionAdvertisement & OrderPosition
            new Facts::OrderPosition { Id = 3, PricePositionId = 3 },
            new Facts::PricePosition { Id = 3, PositionId = 4 },
            new Facts::OrderPositionAdvertisement {Id = 3, PositionId = 5, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 3 },
            new Facts::Position { Id = 4, IsComposite = true }

            )
        .Aggregate(
            // OrderPositionAdvertisement
            new Aggs::OrderPosition { PackagePositionId = 2, ItemPositionId = 3, CategoryId = 10, FirmAddressId = 11 },
            new Aggs::AdvertisementAmountRestriction { PositionId = 2 },

            // OrderPosition
            new Aggs::OrderPosition { PackagePositionId = 3, ItemPositionId = 3 },
            new Aggs::AdvertisementAmountRestriction { PositionId = 3 },
            new Aggs::Position { Id = 3 },

            // OrderPositionAdvertisement & OrderPosition
            new Aggs::OrderPosition { PackagePositionId = 4, ItemPositionId = 5, CategoryId = 10, FirmAddressId = 11 },
            new Aggs::OrderPosition { PackagePositionId = 4, ItemPositionId = 4 },
            new Aggs::AdvertisementAmountRestriction { PositionId = 4 },
            new Aggs::Position { Id = 4 }

            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPriceTest
        => ArrangeMetadataElement.Config
        .Name(nameof(OrderPriceTest))
        .Fact(
            // 1 order, 1 price position
            new Facts::Order { Id = 1 },
            new Facts::OrderPosition { Id = 10,  OrderId = 1, PricePositionId = 10 },
            new Facts::PricePosition { Id = 10, PriceId = 2 },

            // 1 order, 2 price positions
            new Facts::Order { Id = 2 },
            new Facts::OrderPosition { Id = 20, OrderId = 2, PricePositionId = 20 },
            new Facts::OrderPosition { Id = 21, OrderId = 2, PricePositionId = 21 },
            new Facts::PricePosition { Id = 20, PriceId = 3, PositionId = 1 },
            new Facts::PricePosition { Id = 21, PriceId = 3, PositionId = 2 }

            )

        .Aggregate(
            // 1 order, 1 price position
            new Aggs::OrderPrice { OrderId = 1, PriceId = 2 },
            new Aggs::AdvertisementAmountRestriction { PriceId = 2 },
            new Aggs::Order { Id = 1 },
            new Aggs::Period { End = DateTime.MaxValue },

            // 1 order, 2 price positions
            new Aggs::OrderPrice { OrderId = 2, PriceId = 3 },
            new Aggs::AdvertisementAmountRestriction { PriceId = 3, PositionId = 1 },
            new Aggs::AdvertisementAmountRestriction { PriceId = 3, PositionId = 2 },
            new Aggs::Order { Id = 2 },
            new Aggs::Period { End = DateTime.MaxValue }

            );
    }
}
