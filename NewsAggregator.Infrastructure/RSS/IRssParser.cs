using System.IO;

namespace NewsAggregator.Infrastructure.RSS {
    public interface IRssParser
    {
        RssContent Parse(Stream rssStream);
    }
}