using System.IO;

namespace Sociomedia.FeedAggregator.Infrastructure.RSS {
    public interface IRssParser
    {
        RssContent Parse(Stream rssStream);
    }
}