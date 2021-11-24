using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.KafkaIntegration;
using MassTransit;
using MessageBrokerTestApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MessageBrokerTestApp.Implementation
{
    public class KafkaBroker : IMessageBroker
    {
        private readonly IServiceProvider _provider;

        public KafkaBroker(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var bus = _provider.GetRequiredService<IBusControl>();
            return bus.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            var bus = _provider.GetRequiredService<IBusControl>();
            return bus.StopAsync(cancellationToken);
        }

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            var producer = _provider.GetRequiredService<ITopicProducer<TMessage>>();
            return producer.Produce(message, cancellationToken);
        }
    }
}
