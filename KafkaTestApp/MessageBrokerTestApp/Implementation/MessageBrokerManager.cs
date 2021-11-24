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
    public class MessageBrokerManager<TMessage> : IAsyncDisposable where TMessage : class
    {
        private IMessageBroker _broker;

        public async Task<IMessageBroker> StartAsync(string jsonConfigPath)
        {
            Console.WriteLine($"Чтение конфигурации из {jsonConfigPath}...");
            var brokerConfig = JsonConfiguration.Read<MessageBrokerConfiguration>(jsonConfigPath);

            Console.WriteLine($"Настройка клиента...\n{JsonConvert.SerializeObject(brokerConfig, Formatting.Indented)}");
            _broker = MessageBrokerConfigurator.Configure<TMessage>(brokerConfig);

            Console.WriteLine("Запуск клиента...");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await _broker.StartAsync(cts.Token);
            Console.WriteLine("Клиент успешно запущен.");

            return _broker;
        }

        public async Task StopAsync()
        {
            Console.WriteLine("Остановка клиента...");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            await _broker.StopAsync(cts.Token);
            Console.WriteLine("Клиент остановлен");
        }

        public async ValueTask DisposeAsync()
        {
            await this.StopAsync();
            _broker = null;
        }
    }
}
