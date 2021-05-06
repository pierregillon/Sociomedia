using CQRSlite.Domain;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Core.Infrastructure.Logging;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.Medias.Infrastructure
{
    public class MediasRegistry : Registry
    {
        public MediasRegistry(EventStoreConfiguration eventStoreConfiguration)
        {
            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<IEventPublisher>().Use<StructureMapEventPublisher>();

            Scan(scanner => {
                scanner.Assembly("Sociomedia.Medias.Application");
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<ICommandDispatcher>().DecorateAllWith<CommandDispatcherLogger>();

            For<ILoggerFactory>().Use(x => LoggerFactory.Create(x => x.AddConsole()));
            For<ILogger>().Use(x => x.GetInstance<ILoggerFactory>().CreateLogger("test"));
            For<ITypeLocator>().Use<ReflectionTypeLocator>();

            For<IRepository>().Use(context => new Repository(context.GetInstance<IEventStore>()));
            For<IEventStore>().Use<EventStoreOrg>().Singleton();
            For<EventStoreConfiguration>().Use(eventStoreConfiguration).Singleton();
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