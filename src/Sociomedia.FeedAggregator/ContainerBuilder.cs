using Sociomedia.Articles.Infrastructure;
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
                registry.For<Configuration>().Use(configuration).Singleton();
            });
        }
    }
}