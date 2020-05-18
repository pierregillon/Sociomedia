using System;
using System.IO;
using CQRSlite.Domain;
using CQRSlite.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Core.Infrastructure.Logging;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

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

            For<FrenchKeywordDictionary>()
                .Use<FrenchKeywordDictionary>()
                .Ctor<string>()
                .Is(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./Dictionaries/french.csv"))
                .Singleton();

            For<IKeywordDictionary>().Use(x => x.GetInstance<FrenchKeywordDictionary>());

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