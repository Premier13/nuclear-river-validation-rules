﻿using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    // todo: пересмотреть название, тип сообщения более универсален
    public sealed class CorporateBusAggregatableMessage : MessageBase, IAggregatableMessage
    {
        public override Guid Id { get { return Guid.Empty; } }
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<IDataTransferObject> Dtos { get; set; }
    }
}