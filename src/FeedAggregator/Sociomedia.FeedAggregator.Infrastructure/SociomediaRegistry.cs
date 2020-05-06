using CQRSlite.Domain;
using CQRSlite.Events;
using Sociomedia.FeedAggregator.Application;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Domain.Medias;
using Sociomedia.FeedAggregator.Infrastructure.CQRS;
using Sociomedia.FeedAggregator.Infrastructure.Logging;
using Sociomedia.FeedAggregator.Infrastructure.RSS;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class SociomediaRegistry : Registry
    {
        public SociomediaRegistry()
        {
            For<IHtmlParser>().Use<HtmlParser>();
            For<IHtmlPageDownloader>().Use<HtmlPageDownloader>();
            For<IFeedReader>().Use<FeedReader>();
            For<IRssParser>().Use<RssParser>();

            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<IEventPublisher>().Use<StructureMapEventPublisher>();

            Scan(scanner => {
                scanner.AssemblyContainingType(typeof(ICommand));
                scanner.IncludeNamespaceContainingType<ICommand>();
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<IEventPublisher>().DecorateAllWith<EventPublishedLogger>();
            For<ICommandDispatcher>().DecorateAllWith<CommandDispatchedLogger>();

            For<ILogger>().Use<ConsoleLogger>();
            For<ITypeLocator>().Use<ReflectionTypeLocator>();

            For<IMediaFeedFinder>().Use<MediaFeedFinder>();
            For<IRepository>().Use(context => new Repository(context.GetInstance<IEventStore>()));
            For<InMemoryDatabase>().Use<InMemoryDatabase>().Singleton();
            For<IEventStore>().Use<EventStoreOrg>().Singleton();
        }

        private class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry services)
            {
                foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                    foreach (var @interface in type.GetInterfaces()) {
                        services.For(@interface).Use(type);
                    }
                }
            }
        }
    }
}