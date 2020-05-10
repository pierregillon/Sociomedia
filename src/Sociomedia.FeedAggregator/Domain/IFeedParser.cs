using System.IO;

namespace Sociomedia.FeedAggregator.Domain {
    public interface IFeedParser
    {
        FeedContent Parse(Stream rssStream);
    }
}