using System;
using CQRSlite.Domain;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class RssSource : AggregateRoot
    {
        private RssSource() { }

        public RssSource(Uri url) : this()
        {
            ApplyChange(new RssSourceAdded(Guid.NewGuid(), url));
        }

        public DateTime LastSynchronizationDate { get; set; }
        public Uri Url { get; private set; }

        public void Synchronize()
        {
            ApplyChange(new RssSourceSynchronized(Id, DateTime.Now));
        }

        private void Apply(RssSourceAdded @event)
        {
            Id = @event.Id;
            Url = @event.Url;
        }

        private void Apply(RssSourceSynchronized @event)
        {
            LastSynchronizationDate = @event.SynchronizedDate;
        }
    }
}