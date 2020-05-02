using System;
using System.Threading.Tasks;
using NewsAggregator.Application.Commands.AddRssSource;
using NewsAggregator.Application.Commands.SynchronizeRssFeed;
using NewsAggregator.Infrastructure.CQRS;

namespace NewsAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = ContainerBuilder.Build();

            var commandDispatcher = container.GetInstance<ICommandDispatcher>();

            await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));
            await commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            Console.WriteLine("-> ended.");
            Console.ReadKey();
        }
    }
}