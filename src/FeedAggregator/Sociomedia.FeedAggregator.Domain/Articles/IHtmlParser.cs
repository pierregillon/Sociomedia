namespace Sociomedia.Domain.Articles
{
    public interface IHtmlParser
    {
        string ExtractPlainTextArticleContent(string html);
        string ExtractArticleImageUrl(string html);
    }
}