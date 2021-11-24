using System;
using System.Threading.Tasks;
using MassTransit;
using Newtonsoft.Json;

namespace MessageBrokerTestApp.Consumers
{
    public class ExternalLoginRequestConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        public Task Consume(ConsumeContext<TMessage> context)
        {
            Console.WriteLine($"{DateTime.Now}\n{JsonConvert.SerializeObject(context.Message, Formatting.Indented)}\n");
            return Task.CompletedTask;
        }
    }
}
