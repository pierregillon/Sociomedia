using System;

namespace NewsAggregator.Domain.Rss
{
    public class RssSource
    {
        public DateTime LastSynchronizationDate { get; set; }
        public string Id { get; set; }

        public void Synchronized()
        {
            throw new NotImplementedException();
        }
    }
}