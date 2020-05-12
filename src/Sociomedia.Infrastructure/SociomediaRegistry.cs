using System;
using CQRSlite.Domain;
using CQRSlite.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.Application;
using Sociomedia.Domain.Articles;
using Sociomedia.Infrastructure.CQRS;
using Sociomedia.Infrastructure.Logging;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.Infrastructure
{
    public class SociomediaRegistry : Registry
    {
        public SociomediaRegistry(EventStoreConfiguration eventStoreConfiguration)
        {
            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<IEventPublisher>().Use<StructureMapEventPublisher>();

            Scan(scanner => {
                scanner.AssemblyContainingType(typeof(ICommand));
                scanner.IncludeNamespaceContainingType<ICommand>();
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<ICommandDispatcher>().DecorateAllWith<CommandDispatchedLogger>();

            For<ILogger>().Use<ConsoleLogger>();
            For<ITypeLocator>().Use<ReflectionDomainTypeLocator>();

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

    public class EventStoreConfiguration
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 1113;
        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "changeit";

        public Uri Uri => new Uri($"tcp://{Login}:{Password}@{Server}:{Port}");
    }

}