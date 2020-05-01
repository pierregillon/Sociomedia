using System;
using CQRSlite.Domain;

namespace NewsAggregator.Domain.Rss
{
    public class RssSource : AggregateRoot
    {
        public DateTime LastSynchronizationDate { get; set; }

        public void Synchronize()
        {
            ApplyChange(new RssSourceSynchronized(Id, DateTime.Now));
        }

        private void Apply(RssSourceAdded @event)
        {
            Id = @event.Id;
        }

        private void Apply(RssSourceSynchronized @event)
        {
            LastSynchronizationDate = @event.SynchronizedDate;
        }
    }
}