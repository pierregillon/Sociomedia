using CQRSlite.Domain;
using CQRSlite.Events;
using NewsAggregator.Application;
using NewsAggregator.Application.Commands.SynchronizeRssFeed;
using NewsAggregator.Application.Queries;
using NewsAggregator.Domain;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
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

                x.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();

                x.Scan(scanner => {
                    scanner.AssemblyContainingType(typeof(ICommand));
                    scanner.Convention<AllInterfacesConvention>();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                    scanner.AddAllTypesOf(typeof(ICommandHandler<>));
                });

                x.For<IRssSourceFinder>().Use<RssSourceFinder>();
                x.For(typeof(IRepository)).Use(typeof(Repository));

                x.For<InMemoryDatabase>().Singleton();
                x.For<IEventStore>().Use<InMemoryEventStore>().Singleton();
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