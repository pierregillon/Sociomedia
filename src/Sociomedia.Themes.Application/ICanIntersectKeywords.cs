using System.Collections.Generic;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public interface ICanIntersectKeywords
    {
        Keywords IntersectKeywords(ArticleToChallenge article);
        IEnumerable<Article> GetArticles();
    }
}