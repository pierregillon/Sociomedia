using CQRSlite.Domain;
using CQRSlite.Events;
using NewsAggregator.Application;
using NewsAggregator.Application.Queries;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using NewsAggregator.Infrastructure.Logging;
using NewsAggregator.Infrastructure.RSS;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace NewsAggregator
{
    public class ContainerBuilder
    {
        public static Container Build()
        {
            var container = new Container(x => {
                x.For<IHtmlParser>().Use<HtmlParser>();
                x.For<IHtmlPageDownloader>().Use<HtmlPageDownloader>();
                x.For<IRssSourceReader>().Use<RssSourceReader>();
                x.For<IRssParser>().Use<RssParser>();

                x.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();

                x.Scan(scanner => {
                    scanner.AssemblyContainingType(typeof(ICommand));
                    scanner.Convention<AllInterfacesConvention>();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                    scanner.AddAllTypesOf(typeof(ICommandHandler<>));
                });

                x.For<IEventPublisher>().DecorateAllWith<EventPublishedLogger>();
                x.For<ICommandDispatcher>().DecorateAllWith<CommandDispatchedLogger>();

                x.For<ILogger>().Use<ConsoleLogger>();
                x.For<ITypeLocator>().Use<ReflectionTypeLocator>();

                x.For<IRssSourceFinder>().Use<RssSourceFinder>();
                x.For(typeof(IRepository)).Use(context => new Repository(context.GetInstance<IEventStore>()));

                x.For<ReadModelDatabaseFeeder>().Singleton();
                x.For<InMemoryDatabase>().Singleton();

#if !DEBUG
                x.For<IEventStore>().Use<InMemoryEventStore>().Singleton();
#else
                x.For<IEventStore>().Use<EventStoreOrg>().Singleton();
#endif
            });

            return container;
        }
    }

    public class AllInterfacesConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                foreach (var @interface in type.GetInterfaces()) {
                    registry.For(@interface).Use(type);
                }
            }
        }
    }
}