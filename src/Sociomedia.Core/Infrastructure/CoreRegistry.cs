using CQRSlite.Domain;
using CQRSlite.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.Core.Application.Projections;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Core.Infrastructure.Logging;
using StructureMap;

namespace Sociomedia.Core.Infrastructure
{
    public class CoreRegistry : Registry
    {
        public CoreRegistry(EventStoreConfiguration eventStoreConfiguration)
        {
            For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
            For<ICommandDispatcher>().DecorateAllWith<CommandDispatcherLogger>();

            For<IEventPublisher>().Use<StructureMapEventPublisher>();
            For<IEventPublisher>().DecorateAllWith<ChainedEventPublisherDecorator>();

            For<IRepository>().Use(context => new Repository(context.GetInstance<IEventStore>()));

            For<IEventStore>().Use<EventStoreOrg>().Singleton();
            For<IEventStore>().DecorateAllWith<EventStorePublisherDecorator>();
            For<IEventStoreExtended>().Use(x => x.GetInstance<EventStoreOrg>());
            For<EventStoreConfiguration>().Use(eventStoreConfiguration).Singleton();

            For<InMemoryDatabase>().Singleton();
            For<IProjectionLocator>().Use<ProjectionLocator>();

            For<ILogger>().Use<ConsoleLogger>();
        }
    }
}