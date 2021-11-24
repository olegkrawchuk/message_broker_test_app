using System;
using Confluent.Kafka;
using GreenPipes;
using MassTransit;
using MassTransit.KafkaIntegration;
using MessageBrokerTestApp.Consumers;
using MessageBrokerTestApp.Implementation;
using MessageBrokerTestApp.Interfaces;
using MessageBrokerTestApp.Models.ConfigurationModels;
using MessageBrokerTestApp.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageBrokerTestApp.Configurators
{
    public static class MessageBrokerConfigurator
    {

        public static IMessageBroker Configure<TMessage>(MessageBrokerConfiguration configuration) where TMessage : class
        {
            switch (configuration.MessageBroker)
            {
                case MessageBroker.RabbitMQ:
                    return ConfigureRabbitMq<TMessage>(configuration.RabbitMQ);
                case MessageBroker.Kafka:
                    return ConfigureKafka<TMessage>(configuration.Kafka);
            }

            throw new InvalidOperationException("Не выбран брокер сообщений");
        }

        private static IMessageBroker ConfigureKafka<TMessage>(KafkaConfiguration configuration) where TMessage : class
        {
            var services = new ServiceCollection();

            services.AddMassTransit(cfg =>
            {
                cfg.UsingInMemory((context, c) => c.ConfigureEndpoints(context));

                cfg.AddRider(rider =>
                {
                    rider.AddConsumer<ExternalLoginRequestConsumer<TMessage>>();
                    rider.AddProducer<TMessage>(configuration.TopicName);

                    rider.UsingKafka((context, configurator) =>
                    {
                        configurator.SecurityProtocol = configuration.SecurityProtocol;
                        configurator.Host(configuration.Urls, c => c.ConfigureHost(configuration));


                        configurator.TopicEndpoint<TMessage>(configuration.TopicName, groupId: Guid.NewGuid().ToString(), e =>
                        {
                            e.ConfigureConsumer<ExternalLoginRequestConsumer<TMessage>>(context, c => c.UseRetry(a => a.Immediate(100)));
                        });
                    });
                });
            });

            services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Trace));
            var serviceProvider = services.BuildServiceProvider();

            return new KafkaBroker(serviceProvider);
        }

        private static IMessageBroker ConfigureRabbitMq<TMessage>(RabbitMqConfiguration configuration) where TMessage : class
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(configuration.Url, configuration.VirtualHost, h =>
                {
                    h.Username(configuration.Login);
                    h.Password(configuration.Password);
                });

                cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(), ep =>
                {
                    ep.Consumer<ExternalLoginRequestConsumer<TMessage>>();
                });
            });

            return new RabbitMqBroker(bus);
        }

    }
}
