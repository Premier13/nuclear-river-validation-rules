﻿using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlow : MessageFlowBase<MessagesFlow>
    {
        public override Guid Id => new Guid("2B3D30F7-6E59-4510-B680-D7FDD9DEFE0F");

        public override string Description => nameof(MessagesFlow);
    }
}