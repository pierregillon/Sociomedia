using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.Articles.Domain
{
    public interface IFeedReader
    {
        Task<IReadOnlyCollection<ExternalArticle>> ReadArticles(string url);
    }
}