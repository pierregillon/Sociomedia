using CQRSlite.Domain;
using CQRSlite.Events;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.Application.Application;
using Sociomedia.Application.Infrastructure.CQRS;
using Sociomedia.Application.Infrastructure.EventStoring;
using Sociomedia.Application.Infrastructure.Logging;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using ILogger = EventStore.ClientAPI.ILogger;

namespace Sociomedia.Articles.Infrastructure
{
    public class ArticlesRegistry : Registry
    {
        public ArticlesRegistry(EventStoreConfiguration eventStoreConfiguration)
        {
            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<IEventPublisher>().Use<StructureMapEventPublisher>();

            Scan(scanner => {
                scanner.Assembly("Sociomedia.Articles.Application");
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<ICommandDispatcher>().DecorateAllWith<CommandDispatchedLogger>();

            For<IFeedReader>().Use<FeedReader>();
            For<IFeedParser>().Use<FeedParser>();
            For<InMemoryDatabase>().Singleton();
            For<ILogger>().Use<ConsoleLogger>();
            For<ITypeLocator>().Use<ReflectionTypeLocator>();

            For<IRepository>().Use(context => new Repository(context.GetInstance<IEventStore>()));
            For<IEventStore>().Use<EventStoreOrg>().Singleton();
            For<EventStoreConfiguration>().Use(eventStoreConfiguration).Singleton();

            For<IHtmlParser>().Use<HtmlParser>();
            For<IWebPageDownloader>().Use<WebPageDownloader>();
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