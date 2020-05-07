using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Sociomedia.FeedAggregator.Application.SynchronizeAllMediaFeeds;
using Sociomedia.Infrastructure;
using Sociomedia.Infrastructure.CQRS;

namespace Sociomedia.FeedAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = ConfigurationManager<Configuration>.Read();

            var container = ContainerBuilder.Build();

            var eventStore = (EventStoreOrg) container.GetInstance<IEventStore>();

            await eventStore.Connect(
                configuration.EventStore.Server,
                configuration.EventStore.Port,
                configuration.EventStore.Login,
                configuration.EventStore.Password
            );

            await eventStore.StartRepublishingEvents(() => {
                Console.WriteLine("todo !!");
                return Task.CompletedTask;
            });

            var commandDispatcher = container.GetInstance<ICommandDispatcher>();

            do {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                await commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
            } while (true);

            //await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));
            //await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.marianne.net/rss_marianne.xml")));
            //await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("http://rss.liberation.fr/rss/9/")));
            //await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.francetvinfo.fr/france.rss")));

            //await commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
            //await commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            Console.WriteLine("ended.");
            Console.ReadKey();
        }
    }
}