using System;
using System.Linq;

using Microsoft.ServiceBus;

using Quartz;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Security.API.Auth;
using NuClear.Security.API.Context;
using NuClear.Telemetry;
using NuClear.ValidationRules.Hosting.Common;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.ErmFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Events;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusSettingsFactory _serviceBusSettingsFactory;
        private readonly KafkaMessageFlowInfoProvider _kafkaMessageFlowInfoProvider;
        private readonly IQuery _query;

        public ReportingJob(
            ITelemetryPublisher telemetry,
            IServiceBusSettingsFactory serviceBusSettingsFactory,
            KafkaMessageFlowInfoProvider kafkaMessageFlowInfoProvider,
            IUserContextManager userContextManager,
            IUserAuthenticationService userAuthenticationService,
            IUserAuthorizationService userAuthorizationService,
            IJobExecutionObserver jobExecutionObserver,
            IQuery query)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, jobExecutionObserver)
        {
            _kafkaMessageFlowInfoProvider = kafkaMessageFlowInfoProvider;
            _query = query;
            _telemetry = telemetry;
            _serviceBusSettingsFactory = serviceBusSettingsFactory;
        }

        public string Tenant { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            // Костыль.
            // Задача может запускаться без Tenant (для отправки общих метрик) или с ним - для отправки рамера очереди ServiceBus.
            // Надеюсь, после объединения Erm кто-нибудь снова объединит эти задачи.
            if (!string.IsNullOrWhiteSpace(Tenant))
            {
                WithinErrorLogging(ReportServiceBusQueueLength<ErmFactsFlow, PrimaryProcessingQueueLengthIdentity>);
            }
            else
            {
                WithinErrorLogging(ReportMemoryUsage);
                WithinErrorLogging(ReportSqlQueueLength<AggregatesFlow, FinalProcessingAggregateQueueLengthIdentity>);
                WithinErrorLogging(ReportSqlQueueLength<MessagesFlow, MessagesQueueLengthIdentity>);
                WithinErrorLogging(ReportKafkaOffset<AmsFactsFlow, AmsFactsQueueLengthIdentity>);
            }
        }

        private void WithinErrorLogging(Action action)
        {
            action.Invoke();
        }

        private void ReportMemoryUsage()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            _telemetry.Publish<ProcessPrivateMemorySizeIdentity>(process.PrivateMemorySize64);
            _telemetry.Publish<ProcessWorkingSetIdentity>(process.WorkingSet64);
        }

        private void ReportServiceBusQueueLength<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flow = MessageFlowBase<TFlow>.Instance;
            var settings = _serviceBusSettingsFactory.CreateReceiverSettings(flow);
            var manager = NamespaceManager.CreateFromConnectionString(settings.ConnectionString);
            var subscription = manager.GetSubscription(settings.TransportEntityPath, flow.Id.ToString());
            _telemetry.Publish<TTelemetryIdentity>(subscription.MessageCountDetails.ActiveMessageCount);
        }

        private void ReportSqlQueueLength<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flowId = MessageFlowBase<TFlow>.Instance.Id;
            var eventCount = _query.For<EventRecord>().Count(x => x.Flow == flowId);
            _telemetry.Publish<TTelemetryIdentity>(eventCount);
        }

        private void ReportKafkaOffset<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flow = MessageFlowBase<TFlow>.Instance;

            var size = _kafkaMessageFlowInfoProvider.GetFlowSize(flow);
            var processedSize = _kafkaMessageFlowInfoProvider.GetFlowProcessedSize(flow);

            _telemetry.Publish<TTelemetryIdentity>(size - processedSize);
        }

        private sealed class MessagesQueueLengthIdentity : TelemetryIdentityBase<MessagesQueueLengthIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesQueueLengthIdentity);
        }

        private sealed class AmsFactsQueueLengthIdentity : TelemetryIdentityBase<AmsFactsQueueLengthIdentity>
        {
            public override int Id => 0;

            public override string Description => nameof(AmsFactsQueueLengthIdentity);
        }
    }
}
