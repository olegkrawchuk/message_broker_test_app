using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageBrokerTestApp.Implementation;
using MessageBrokerTestApp.Interfaces;
using MessageBrokerTestApp.Models.MessageModels;

namespace MessageBrokerTestApp
{
    internal class Program
    {
        private static void Cancel(CancellationTokenSource cts)
        {
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            cts.Cancel();
        }


        private static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            // отдельный поток, который отменит отправку при нажании Escape
            _ = Task.Run(() => Cancel(cts), default);

            try
            {
                await using var brokerManager = new MessageBrokerManager();

                var broker = await brokerManager.StartAsync<ExternalLoginRequestViewModel>("messageBrokerConfig.json", cts.Token);

                var (countMessages, delay) = ParseArgs(args, countDefault: 100, delayDefault: 3000);

                Console.WriteLine($"Кол-во сообщений: {countMessages}");
                Console.WriteLine($"Задержка между сообщениями: {delay}ms");

                await PublishCommandAsync(broker, countMessages, delay, cts.Token);

                if (!cts.IsCancellationRequested)
                {
                    while (Console.ReadKey().Key != ConsoleKey.Escape) { }
                    cts.Cancel();
                }

                Console.WriteLine("Приложение завершило работу");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                cts.Cancel();
            }
        }

        private static async Task PublishCommandAsync(IMessageBroker broker, int countMessages, int delay, CancellationToken cancellationToken)
        {
            var authenticationStates = Enum.GetValues(typeof(AuthenticationState)) as AuthenticationState[];
            var rnd = new Random();

            var messages = Enumerable.Range(0, countMessages).Select(i => new ExternalLoginRequestViewModel
            {
                Id = i.ToString(),
                Status = authenticationStates![rnd.Next(0, authenticationStates.Length - 1)],
                ExpiresIn = rnd.Next(60, 3600)
            });

            //var messages = Enumerable.Range(0, countMessages).Select(i => new SimpleMessage
            //{
            //    Text = $"TEST Message {i}"
            //});

            try
            {
                foreach (var m in messages)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException();

                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));
                        await broker.PublishAsync(m, cts.Token);
                        Console.WriteLine($"Отправлено! Id={m.Id}");
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw;

                        Console.WriteLine("Истекло время отправки сообщения");
                    }

                }

                Console.WriteLine("Все сообщения отправлены!");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Отправка отменена пользователем");
            }
        }

        private static (int count, int delay) ParseArgs(string[] args, int countDefault, int delayDefault)
        {
            var indexCount = Array.IndexOf(args, "-c");
            if (indexCount == -1)
            {
                indexCount = Array.IndexOf(args, "--count");
            }

            var indexDelay = Array.IndexOf(args, "-d");
            if (indexCount == -1)
            {
                indexDelay = Array.IndexOf(args, "--delay");
            }

            var countMessages = indexCount == -1 ? countDefault : int.Parse(args[indexCount + 1]);
            var delay = indexDelay == -1 ? delayDefault : int.Parse(args[indexDelay + 1]);

            return (countMessages, delay);
        }
    }
}
