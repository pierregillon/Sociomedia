using System;

namespace Sociomedia.DomainEvents.RssSource
{
    public class RssSourceAdded : DomainEvent
    {
        public RssSourceAdded(Guid id, Uri url) : base(id)
        {
            Id = id;
            Url = url;
        }

        public Uri Url { get; }
    }
}