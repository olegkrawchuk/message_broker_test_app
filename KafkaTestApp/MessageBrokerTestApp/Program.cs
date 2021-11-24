using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using MessageBrokerTestApp.Implementation;
using MessageBrokerTestApp.Implementation.Commands;
using MessageBrokerTestApp.Interfaces;
using MessageBrokerTestApp.Models.MessageModels;

namespace MessageBrokerTestApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await using var brokerManager = new MessageBrokerManager<ExternalLoginRequestViewModel>();
            var broker = await brokerManager.StartAsync("messageBrokerConfig.json");

            _ = (await Parser.Default.ParseArguments<HelpCommand, PublishCommand>(args)
                    .WithParsed<HelpCommand>(command => command.Execute())
                    .WithParsedAsync<PublishCommand>(command => command.ExecuteAsync(broker, PublishCommandAsync)))
                    .WithNotParsedAsync(command => new PublishCommand().ExecuteAsync(broker, PublishCommandAsync))
                ;

            Console.ReadLine();
        }


        private static async Task PublishCommandAsync(IMessageBroker broker, int countMessages, int delay)
        {
            var authenticationStates = Enum.GetValues(typeof(AuthenticationState)) as AuthenticationState[];
            var rnd = new Random();

            var messages = Enumerable.Range(0, countMessages).Select(i => new ExternalLoginRequestViewModel
            {
                Status = authenticationStates![rnd.Next(0, authenticationStates.Length - 1)],
                ExpiresIn = rnd.Next(60, 3600)
            });

            foreach (var m in messages)
            {
                await broker.PublishAsync(m);
                await Task.Delay(delay);
            }

            Console.WriteLine("Все сообщения отправлены!");
        }
    }
}
