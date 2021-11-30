using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageBrokerTestApp.Helpers
{
    public class KafkaJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            var serialized = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()));
        }
    }
}
