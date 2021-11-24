using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MessageBrokerTestApp.Models.ConfigurationModels
{
    public class KafkaSaslConfiguration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SaslMechanism? SaslMechanism { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
