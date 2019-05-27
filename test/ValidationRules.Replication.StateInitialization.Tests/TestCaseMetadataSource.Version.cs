﻿using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Aggregates.FirmRules;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;
using Cache = NuClear.ValidationRules.Storage.Model.Messages.Cache;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Version
        => ArrangeMetadataElement.Config
            .Name(nameof(Version))
            .Aggregate(new Order.FmcgCutoutPosition()) // fixme: нужны тесты на новые проверки!
            .Message(new Version(), new Version.ErmState(), new Version.AmsState(), new Cache.ValidationResult())
            .Ignored();
    }
}
