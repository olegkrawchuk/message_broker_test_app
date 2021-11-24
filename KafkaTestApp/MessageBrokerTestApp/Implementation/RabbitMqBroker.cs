using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MessageBrokerTestApp.Interfaces;

namespace MessageBrokerTestApp.Implementation
{
    public class RabbitMqBroker : IMessageBroker
    {
        private readonly IBusControl _bus;

        public RabbitMqBroker(IBusControl bus)
        {
            _bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken = default) =>
            _bus.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = default) =>
            _bus.StopAsync(cancellationToken);

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class =>
            _bus.Publish(message, cancellationToken);
    }
}
