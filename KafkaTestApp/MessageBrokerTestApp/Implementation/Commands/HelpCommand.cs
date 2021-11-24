using System;
using CommandLine;

namespace MessageBrokerTestApp.Implementation.Commands
{
    [Verb("?", HelpText = "Display help")]
    public class HelpCommand
    {
        public void Execute()
        {
            Console.WriteLine("Using:");
            Console.WriteLine("\t MessageBrokerTestApp -c 10 -d 2000\n");
            Console.WriteLine("Parameters:");
            Console.WriteLine("\tc|count\tcount publishing messages");
            Console.WriteLine("\td|delay\tdelay between publishing messages");
        }
    }
}
