using System;
using System.Threading.Tasks;
using CommandLine;
using MessageBrokerTestApp.Interfaces;

namespace MessageBrokerTestApp.Implementation.Commands
{
    [Verb("publish", HelpText = "Publish message command")]
    public class PublishCommand
    {
        [Option('c', "count", Required = false, Default = 50, HelpText = "Count publishing messages")]
        public int? Count { get; set; }


        [Option('d', "delay", Required = false, Default = 2000, HelpText = "Delay between publishing")]
        public int? Delay { get; set; }


        public Task ExecuteAsync(IMessageBroker broker, Func<IMessageBroker, int, int, Task> action)
        {
            return action(broker, Count ?? 5, Delay ?? 2000);
        }
    }
}
