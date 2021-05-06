using Microsoft.Extensions.Logging;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.FeedAggregator.Application;
using StructureMap;

namespace Sociomedia.FeedAggregator
{
    public class ContainerBuilder
    {
        public static Container Build(Configuration configuration)
        {
            return new Container(registry => {
                registry.IncludeRegistry(new CoreRegistry(configuration.EventStore));
                registry.IncludeRegistry<ArticlesRegistry>();
                registry.For<Aggregator>().Singleton();
                registry.For<ITypeLocator>().Use<ReflectionTypeLocator<ArticleEvent>>();
                registry.For<IEventBus>().Use<EventStoreOrgBus>().Singleton();
                registry.For<IEventPositionRepository>().Use<EventPositionRepository>();
                registry.For<Configuration>().Use(configuration).Singleton();
            });
        }
    }
}