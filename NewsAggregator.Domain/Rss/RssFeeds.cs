using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.Domain.Rss
{
    public class RssFeeds : IReadOnlyCollection<RssFeed>
    {
        private readonly List<RssFeed> _feeds;

        public RssFeeds(IEnumerable<RssFeed> feeds)
        {
            _feeds = feeds.ToList();
        }

        public IEnumerator<RssFeed> GetEnumerator()
        {
            return _feeds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _feeds.Count;
    }
}