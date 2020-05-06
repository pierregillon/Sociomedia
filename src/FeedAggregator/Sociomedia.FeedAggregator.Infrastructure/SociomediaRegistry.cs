using CQRSlite.Domain;
using CQRSlite.Events;
using Lamar;
using Sociomedia.FeedAggregator.Application;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Domain.Medias;
using Sociomedia.FeedAggregator.Infrastructure.CQRS;
using Sociomedia.FeedAggregator.Infrastructure.Logging;
using Sociomedia.FeedAggregator.Infrastructure.RSS;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class SociomediaRegistry : ServiceRegistry
    {
        public SociomediaRegistry()
        {
            For<IHtmlParser>().Use<HtmlParser>();
            For<IHtmlPageDownloader>().Use<HtmlPageDownloader>();
            For<IRssSourceReader>().Use<RssSourceReader>();
            For<IRssParser>().Use<RssParser>();

            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<IEventPublisher>().Use<StructureMapEventPublisher>();

            Scan(scanner => {
                scanner.AssemblyContainingType(typeof(ICommand));
                scanner.IncludeNamespaceContainingType<ICommand>();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<IEventPublisher>().DecorateAllWith<EventPublishedLogger>();
            For<ICommandDispatcher>().DecorateAllWith<CommandDispatchedLogger>();

            For<ILogger>().Use<ConsoleLogger>();
            For<ITypeLocator>().Use<ReflectionTypeLocator>();

            For<IRssSourceFinder>().Use<RssSourceFinder>();
            For<IRepository>().Use(context => new Repository(context.GetInstance<IEventStore>()));
            For<InMemoryDatabase>().Use<InMemoryDatabase>().Singleton();
            For<IEventStore>().Use<EventStoreOrg>().Singleton();

            Injectable<IEventStore>();
            Injectable<ILogger>();
            Injectable<IHtmlPageDownloader>();
            Injectable<IRssSourceReader>();
        }
    }
}