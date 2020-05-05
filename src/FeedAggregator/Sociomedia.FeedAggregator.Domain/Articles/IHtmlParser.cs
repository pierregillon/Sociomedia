namespace Sociomedia.FeedAggregator.Domain.Articles
{
    public interface IHtmlParser
    {
        string ExtractPlainTextArticleContent(string html);
    }
}