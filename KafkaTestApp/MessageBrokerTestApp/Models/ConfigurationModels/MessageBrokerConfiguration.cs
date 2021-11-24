using MessageBrokerTestApp.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MessageBrokerTestApp.Models.ConfigurationModels
{
    public class MessageBrokerConfiguration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageBroker MessageBroker { get; set; }

        public KafkaConfiguration Kafka { get; set; }
        public RabbitMqConfiguration RabbitMQ { get; set; }
    }
}
