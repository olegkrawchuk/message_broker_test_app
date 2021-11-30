using System;
using System.Threading;
using System.Threading.Tasks;
using MessageBrokerTestApp.Interfaces;
using Confluent.Kafka;
using MessageBrokerTestApp.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace MessageBrokerTestApp.Implementation
{
    public class KafkaBroker : IMessageBroker
    {
        // private readonly IServiceProvider _provider;
        private readonly ClientConfig _clientConfig;
        private readonly Dictionary<string, Type> _topics;

        public KafkaBroker(ClientConfig clientConfig, Dictionary<string, Type> topics)
        {
            _clientConfig = clientConfig;
            _topics = topics;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            var messageType = typeof(TMessage);
            var topic = _topics.FirstOrDefault(t => t.Value == messageType).Key;
            if (topic == null)
                throw new ArgumentException($"Topic for {messageType} not found");


            using var producer = new ProducerBuilder<Null, TMessage>(_clientConfig)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();

            await producer.ProduceAsync(topic, new Message<Null, TMessage> { Value = message }, cancellationToken);
            producer.Flush(cancellationToken);
        }
    }
}
