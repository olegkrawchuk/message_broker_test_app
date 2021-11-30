using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MessageBrokerTestApp.Consumers;
using MessageBrokerTestApp.Implementation;
using MessageBrokerTestApp.Interfaces;
using MessageBrokerTestApp.Models.ConfigurationModels;

namespace MessageBrokerTestApp.Configurators
{
    public static class MessageBrokerConfigurator
    {

        public static IMessageBroker Configure<TMessage>(MessageBrokerConfiguration configuration, CancellationToken consumerCancellationToken) where TMessage : class
        {
            return ConfigureKafka<TMessage>(configuration.Kafka, consumerCancellationToken);
        }

        private static IMessageBroker ConfigureKafka<TMessage>(KafkaConfiguration configuration, CancellationToken consumerCancellationToken) where TMessage : class
        {
            var clientConfig = new ClientConfig
            {
                BootstrapServers = configuration.Url,
                ClientId = Dns.GetHostName(),
                SecurityProtocol = configuration.SecurityProtocol
            };

            clientConfig.ConfigureAuth(configuration);


            var consumerConfig = new ConsumerConfig(clientConfig) { GroupId = Guid.NewGuid().ToString() };

            Task.Factory.StartNew(
                    async token => await new KafkaConsumer()
                        .Consume<TMessage>(consumerConfig, configuration.TopicName, consumerCancellationToken)
                        .ContinueWith(t => Console.WriteLine("Консьюмер остановлен!"), consumerCancellationToken),
                    consumerCancellationToken,
                    TaskCreationOptions.LongRunning)
                .ConfigureAwait(false);


            return new KafkaBroker(clientConfig, new Dictionary<string, Type>
            {
                { configuration.TopicName, typeof(TMessage) }
            });
        }

        //private static IMessageBroker ConfigureRabbitMq<TMessage>(RabbitMqConfiguration configuration) where TMessage : class
        //{
        //    throw new NotSupportedException(
        //        "Тестирование RabbitMQ временно не доступно. Обратитесь к разработчикам приложения");

        //    var factory = new ConnectionFactory
        //    {
        //        UserName = configuration.Login,
        //        Password = configuration.Password,
        //        VirtualHost = configuration.VirtualHost,
        //        HostName = configuration.Url,
        //        DispatchConsumersAsync = true,
        //        AutomaticRecoveryEnabled = true,
        //        NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        //    };

        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    var consumer = new AsyncEventingBasicConsumer(channel);

        //    consumer.Received += async (ch, ea) =>
        //    {
        //        var message = JsonConvert.DeserializeObject<object>(Encoding.UTF8.GetString(ea.Body.ToArray()));

        //        channel.BasicAck(ea.DeliveryTag, false);
        //        await Task.Yield();
        //    };

        //    var consumerTag = channel.BasicConsume("", false, consumer);

        //    channel.Close();
        //    connection.Close();

        //    return new RabbitMqBroker(null);
        //}

    }
}
