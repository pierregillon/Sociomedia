using System;
using System.Threading.Tasks;

namespace NewsAggregator.Domain.Rss
{
    public interface IRssFeedReader
    {
        Task<RssFeeds> ReadNewFeeds(string url, DateTime? from);
    }
}