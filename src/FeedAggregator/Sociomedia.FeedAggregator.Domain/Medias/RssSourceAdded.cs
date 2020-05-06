using System;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class RssSourceAdded : DomainEvents.RssSource.RssSourceAdded, IDomainEvent
    {
        public RssSourceAdded(Guid id, Uri url) : base(id, url) { }
    }
}