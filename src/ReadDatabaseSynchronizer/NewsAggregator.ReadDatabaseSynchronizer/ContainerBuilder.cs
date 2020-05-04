using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using StructureMap;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    public class ContainerBuilder
    {
        public static IContainer Build()
        {
            return new Container(configuration => {
                configuration.For<EventStoreOrg>().Singleton();
                configuration.For<IEventPublisher>().Use<StructureMapEventPublisher>();
                configuration.For<ITypeLocator>().Use<ReflectionTypeLocator>();
                configuration.For<ILogger>().Use<ConsoleLogger>();
            });
        }
    }
}