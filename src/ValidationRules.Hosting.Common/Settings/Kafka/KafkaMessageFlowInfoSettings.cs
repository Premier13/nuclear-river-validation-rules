﻿using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace NuClear.ValidationRules.Hosting.Common.Settings.Kafka
{
    internal sealed class KafkaMessageFlowInfoSettings : IKafkaMessageFlowInfoSettings
    {
        public Dictionary<string, object> Config { get; set; }
        public TopicPartition TopicPartition { get; set; }
        public TimeSpan InfoTimeout { get; set; }
    }
}