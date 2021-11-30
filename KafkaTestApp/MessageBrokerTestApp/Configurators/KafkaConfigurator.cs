using System.IO;
using System.Reflection;
using MessageBrokerTestApp.Models.ConfigurationModels;
using Confluent.Kafka;

namespace MessageBrokerTestApp.Configurators
{
    public static class KafkaConfigurator
    {
        public static void ConfigureAuth(this ClientConfig c, KafkaConfiguration configuration)
        {
            switch (configuration.SecurityProtocol)
            {
                case SecurityProtocol.SaslSsl:
                    c.ConfigureSsl(configuration.Ssl);
                    c.ConfigureSasl(configuration.Sasl);
                    break;

                case SecurityProtocol.Ssl:
                    c.ConfigureSsl(configuration.Ssl);
                    break;

                case SecurityProtocol.SaslPlaintext:
                    c.ConfigureSasl(configuration.Sasl);
                    break;
            }
        }

        private static void ConfigureSsl(this ClientConfig c, KafkaSslConfiguration configuration)
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", string.Empty);

            var keyLocation = Path.IsPathRooted(configuration.KeyLocation)
                ? configuration.KeyLocation
                : Path.Combine(currentDirectory, configuration.KeyLocation);
            c.SslKeyLocation = keyLocation;

            c.SslKeyPassword = configuration.KeyPassword;

            var certificateLocation = Path.IsPathRooted(configuration.CertificateLocation)
                ? configuration.CertificateLocation
                : Path.Combine(currentDirectory, configuration.CertificateLocation);
            c.SslCertificateLocation = certificateLocation;

            var caLocation = Path.IsPathRooted(configuration.CaLocation)
                ? configuration.CaLocation
                : Path.Combine(currentDirectory, configuration.CaLocation);
            c.SslCaLocation = caLocation;
        }

        private static void ConfigureSasl(this ClientConfig c, KafkaSaslConfiguration configuration)
        {
            c.SaslMechanism = configuration.SaslMechanism;
            c.SaslUsername = configuration.Username;
            c.SaslPassword = configuration.Password;
        }
    }
}
