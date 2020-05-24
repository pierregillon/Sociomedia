using System.Collections.Generic;
using System.IO;

namespace Sociomedia.Articles.Domain.Feeds
{
    public interface IFeedParser
    {
        IReadOnlyCollection<FeedItem> Parse(Stream stream);
    }
}