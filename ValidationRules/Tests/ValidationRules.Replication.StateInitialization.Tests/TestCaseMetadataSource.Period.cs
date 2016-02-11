﻿using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Erm = NuClear.ValidationRules.Domain.Model.Erm;
    using Facts = NuClear.ValidationRules.Domain.Model.Facts;
    using Aggregates = NuClear.ValidationRules.Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SingleOrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(SingleOrderPeriod))
                                     .Aggregate(
                                                new Aggregates::Period
                                                    {
                                                        Id = 1,
                                                        Start = DateTime.Parse("2011-01-01T00:00:00"),
                                                        End = DateTime.Parse("2011-05-01T00:00:00"),
                                                    },
                                                new Aggregates::Period
                                                    {
                                                        Id = 2,
                                                        Start = DateTime.Parse("2011-05-01T00:00:00"),
                                                        End = DateTime.MaxValue,
                                                    })
                                     .Fact(
                                           new Facts::Order
                                               {
                                                   BeginDistributionDate = DateTime.Parse("2011-01-01T00:00:00"),
                                                   EndDistributionDateFact = DateTime.Parse("2011-05-01T00:00:00")
                                               });

        private static ArrangeMetadataElement SinglePricePeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(SinglePricePeriod))
                                     .Aggregate(
                                                new Aggregates::Period
                                                {
                                                    Id = 1,
                                                    Start = DateTime.Parse("2011-01-01T00:00:00"),
                                                    End = DateTime.MaxValue,
                                                })
                                     .Fact(
                                           new Facts::Price
                                           {
                                               BeginDate = DateTime.Parse("2011-01-01T00:00:00"),
                                           });
    }
}
