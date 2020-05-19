using System.IO;

namespace Sociomedia.Articles.Domain
{
    public interface IFeedParser
    {
        FeedContent Parse(Stream rssStream);
    }
}