using System;
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

            await aggregator.StartAggregation();

            Console.WriteLine("Ended.");
            Console.ReadKey();
        }
    }
}