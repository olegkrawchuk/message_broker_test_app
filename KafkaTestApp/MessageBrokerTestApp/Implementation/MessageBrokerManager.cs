using MessageBrokerTestApp.Configurators;
using MessageBrokerTestApp.Helpers;
using MessageBrokerTestApp.Models.ConfigurationModels;
using Newtonsoft.Json;
using System.Threading;
using System;
using System.Threading.Tasks;
using MessageBrokerTestApp.Interfaces;

namespace MessageBrokerTestApp.Implementation
{
    public class MessageBrokerManager : IAsyncDisposable
    {
        private IMessageBroker _broker;

        public async Task<IMessageBroker> StartAsync<TMessage>(string jsonConfigPath, CancellationToken consumerCancellationToken) where TMessage : class
        {
            Console.WriteLine($"Чтение конфигурации из {jsonConfigPath}...");
            var brokerConfig = JsonConfiguration.Read<MessageBrokerConfiguration>(jsonConfigPath);

            Console.WriteLine($"Настройка клиента...\n{JsonConvert.SerializeObject(brokerConfig, Formatting.Indented)}");

            _broker = MessageBrokerConfigurator.Configure<TMessage>(brokerConfig, consumerCancellationToken);

            Console.WriteLine("Запуск клиента...");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await _broker.StartAsync(cts.Token);
            Console.WriteLine("Клиент успешно запущен.");

            return _broker;
        }

        public async Task StopAsync()
        {
            Console.WriteLine("Остановка клиента...");
            if (_broker != null)
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                await _broker.StopAsync(cts.Token);
            }
            Console.WriteLine("Клиент остановлен");
        }

        public async ValueTask DisposeAsync()
        {
            await this.StopAsync();
            _broker = null;
        }
    }
}
