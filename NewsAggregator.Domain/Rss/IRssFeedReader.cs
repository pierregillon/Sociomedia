using System.Threading.Tasks;

namespace NewsAggregator.Domain.Rss {
    public interface IRssFeedReader
    {
        Task<RssFeeds> Read(string url);
    }
}