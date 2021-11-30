using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MessageBrokerTestApp.Helpers;
using Newtonsoft.Json;

namespace MessageBrokerTestApp.Consumers
{
    public class KafkaConsumer
    {
        public async Task Consume<TMessage>(ConsumerConfig config, string topicName, CancellationToken cancellationToken = default)
        {
            using var consumer = new ConsumerBuilder<Ignore, TMessage>(config)
                .SetValueDeserializer(new KafkaJsonSerializer<TMessage>())
                .Build();

            consumer.Subscribe(topicName);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Принято: \n{JsonConvert.SerializeObject(message.Message.Value, Formatting.Indented)}\n");
                    }
                    catch (ConsumeException ex)
                    {
                        Console.WriteLine($"Consume Error: {ex}");
                    }

                    await Task.Delay(20, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }
}
