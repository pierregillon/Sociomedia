using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = ConfigurationManager<Configuration>.Read();

            var container = ContainerBuilder.Build(configuration);

            var aggregator = container.GetInstance<Aggregator>();

            var source = new CancellationTokenSource();

            await aggregator.StartAggregation(source.Token);

            WaitForExit();

            Console.WriteLine("Stopping");

            source.Cancel();

            Console.WriteLine("Stopped");
        }

        private static void WaitForExit()
        {
            var exitEvent = new ManualResetEvent(false);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                exitEvent.Set();
            };

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
        }
    }
}