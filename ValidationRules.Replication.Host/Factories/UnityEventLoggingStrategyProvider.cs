﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.OperationsLogging.API;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication.Host.Factories
{
    public sealed class UnityEventLoggingStrategyProvider : IEventLoggingStrategyProvider
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IServiceBusSettingsFactory _settingsFactory;

        public UnityEventLoggingStrategyProvider(IUnityContainer unityContainer, IServiceBusSettingsFactory settingsFactory)
        {
            _unityContainer = unityContainer;
            _settingsFactory = settingsFactory;
        }

        public IReadOnlyCollection<IEventLoggingStrategy<TEvent>> Get<TEvent>(IReadOnlyCollection<TEvent> events)
        {
            var flows = new Dictionary<IMessageFlow, IFlowAspect<TEvent>>
                {
                    { AggregatesFlow.Instance, _unityContainer.Resolve<AggregatesFlowAspect<TEvent>>() },
                    { MessagesFlow.Instance, _unityContainer.Resolve<MessagesFlowAspect<TEvent>>() }
                };

            return flows.Select(flow => new { Strategy = ResolveServiceBusStrategy<TEvent>(flow.Key), Aspect = flow.Value })
                        .Select(x => DecorateStrategy(x.Strategy, x.Aspect))
                        .ToArray();
        }

        private IEventLoggingStrategy<TEvent> ResolveServiceBusStrategy<TEvent>(IMessageFlow flow)
        {
            var serviceBusSettings = _settingsFactory.CreateSenderSettings(flow);
            var strategy = _unityContainer.Resolve<SessionlessServiceBusEventLoggingStrategy<TEvent>>(new DependencyOverride<IServiceBusMessageSenderSettings>(serviceBusSettings));
            return strategy;
        }

        private IEventLoggingStrategy<TEvent> DecorateStrategy<TEvent>(IEventLoggingStrategy<TEvent> strategy, IFlowAspect<TEvent> flow)
        {
            return new EventLoggingStrategyDecorator<TEvent>(strategy, flow);
        }

        private class AggregatesFlowAspect<TEvent> : IFlowAspect<TEvent>
        {
            public bool ShouldEventBeLogged(TEvent @event)
                => !(@event is AggregatesStateIncrementedEvent || @event is AggregatesDelayLoggedEvent || @event is ResultOutdatedEvent || @event is ResultPartiallyOutdatedEvent);

            public void ReportMessageLoggedCount(long count)
            {
            }
        }

        private class MessagesFlowAspect<TEvent> : IFlowAspect<TEvent>
        {
            public bool ShouldEventBeLogged(TEvent @event)
                => @event is AggregatesStateIncrementedEvent || @event is AggregatesDelayLoggedEvent || @event is ResultOutdatedEvent || @event is ResultPartiallyOutdatedEvent;

            public void ReportMessageLoggedCount(long count)
            {
            }
        }
    }
}
