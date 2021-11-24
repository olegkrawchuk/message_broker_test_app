using MassTransit.KafkaIntegration;
using System.IO;
using System.Reflection;
using MessageBrokerTestApp.Models.ConfigurationModels;
using Confluent.Kafka;

namespace MessageBrokerTestApp.Configurators
{
    public static class KafkaConfigurator
    {
        public static void ConfigureHost(this IKafkaHostConfigurator configurator, KafkaConfiguration configuration)
        {
            switch (configuration.SecurityProtocol)
            {
                case SecurityProtocol.SaslSsl:
                    configurator.UseSsl(c => c.ConfigureSsl(configuration.Ssl));
                    configurator.UseSasl(c => c.ConfigureSasl(configuration.Sasl));
                    break;

                case SecurityProtocol.Ssl:
                    configurator.UseSsl(c => c.ConfigureSsl(configuration.Ssl));
                    break;

                case SecurityProtocol.SaslPlaintext:
                    configurator.UseSasl(c => c.ConfigureSasl(configuration.Sasl));
                    break;
            }
        }

        private static void ConfigureSsl(this IKafkaSslConfigurator c, KafkaSslConfiguration configuration)
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", string.Empty);

            var keyLocation = Path.IsPathRooted(configuration.KeyLocation)
                ? configuration.KeyLocation
                : Path.Combine(currentDirectory, configuration.KeyLocation);
            c.KeyLocation = keyLocation;

            c.KeyPassword = configuration.KeyPassword;

            var certificateLocation = Path.IsPathRooted(configuration.CertificateLocation)
                ? configuration.CertificateLocation
                : Path.Combine(currentDirectory, configuration.CertificateLocation);
            c.CertificateLocation = certificateLocation;

            var caLocation = Path.IsPathRooted(configuration.CaLocation)
                ? configuration.CaLocation
                : Path.Combine(currentDirectory, configuration.CaLocation);
            c.CaLocation = caLocation;
        }

        private static void ConfigureSasl(this IKafkaSaslConfigurator c, KafkaSaslConfiguration configuration)
        {
            c.Mechanism = configuration.SaslMechanism;
            c.Username = configuration.Username;
            c.Password = configuration.Password;
        }
    }
}
