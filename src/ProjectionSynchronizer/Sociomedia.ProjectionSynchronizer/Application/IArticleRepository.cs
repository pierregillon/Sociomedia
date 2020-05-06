using System.Threading.Tasks;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IArticleRepository
    {
        Task AddArticle(ArticleTable article);
        Task AddKeywords(KeywordTable keyword);
    }
}