using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace MessageBrokerTestApp.Models.ConfigurationModels
{
    public class KafkaConfiguration
    {
        public IReadOnlyList<string> Urls { get; set; }
        public string TopicName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SecurityProtocol? SecurityProtocol { get; set; }

        public KafkaSslConfiguration Ssl { get; set; }
        public KafkaSaslConfiguration Sasl { get; set; }
    }

}
