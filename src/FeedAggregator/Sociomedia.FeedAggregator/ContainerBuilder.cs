using Sociomedia.FeedAggregator.Infrastructure;
using StructureMap;

namespace Sociomedia.FeedAggregator
{
    public class ContainerBuilder
    {
        public static Container Build()
        {
            return new Container(registry => registry.IncludeRegistry<SociomediaRegistry>());
        }
    }
}