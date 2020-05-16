using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Infrastructure.EventStoring;
using StructureMap;

namespace Sociomedia.FeedAggregator
{
    public class ContainerBuilder
    {
        public static Container Build(Configuration configuration)
        {
            return new Container(registry => {
                registry.IncludeRegistry(new ArticlesRegistry(configuration.EventStore));
                registry.For<Aggregator>().Singleton();
                registry.For<IEventBus>().Use<EventStoreOrgBus>().Singleton();
                registry.For<Configuration>().Use(configuration).Singleton();
            });
        }
    }
}