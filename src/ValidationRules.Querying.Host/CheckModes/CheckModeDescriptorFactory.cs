﻿using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Erm;
using NuClear.ValidationRules.Storage.Model.Messages;
using System.Linq;

namespace NuClear.ValidationRules.Querying.Host.CheckModes
{
    public sealed class CheckModeDescriptorFactory
    {
        private static readonly IReadOnlyDictionary<CheckMode, Dictionary<MessageTypeCode, RuleSeverityLevel>> CheckModes =
            CheckModeRegistry.Map.SelectMany(x => x.Item2.Select(y => new { MessageTypeCode = x.Item1, CheckMode = y.Key, RuleSeverityLevel = y.Value }))
                         .GroupBy(x => x.CheckMode)
                         .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.MessageTypeCode, y => y.RuleSeverityLevel));

        public ICheckModeDescriptor GetDescriptorFor(CheckMode checkMode)
        {
            if (!CheckModes.TryGetValue(checkMode, out var checkModeRules))
            {
                throw new ArgumentException($"Check mode {checkMode} nas no one rule", nameof(checkModeRules));
            }

            return new CheckModeDescriptor(checkMode, checkModeRules);
        }

        private sealed class CheckModeDescriptor : ICheckModeDescriptor
        {
            public CheckModeDescriptor(CheckMode checkMode, IReadOnlyDictionary<MessageTypeCode, RuleSeverityLevel> checkModeRules)
            {
                CheckMode = checkMode;
                Rules = checkModeRules;
            }

            public IReadOnlyDictionary<MessageTypeCode, RuleSeverityLevel> Rules { get; }

            public CheckMode CheckMode { get; }

            public DateTime GetValidationPeriodStart(Order order)
            {
                const int OrderStateOnTermination = 4;
                if (CheckMode == CheckMode.SingleForApprove && order.WorkflowStepId == OrderStateOnTermination)
                {
                    return order.AgileDistributionEndFactDate.AddSeconds(1);
                }

                return order.AgileDistributionStartDate;
            }
        }
    }
}
