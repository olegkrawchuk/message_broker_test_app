using System.Threading;
using System.Threading.Tasks;

namespace MessageBrokerTestApp.Interfaces
{
    public interface IMessageBroker
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
    }
}
