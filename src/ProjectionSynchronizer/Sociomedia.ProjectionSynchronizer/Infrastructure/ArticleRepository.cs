using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ProjectionSynchronizer.Application.EventListeners;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure {
    public class ArticleRepository : IArticleRepository
    {
        private readonly DbConnectionReadModel _connection;

        public ArticleRepository(DbConnectionReadModel connection)
        {
            _connection = connection;
        }

        public async Task AddArticle(ArticleTable article)
        {
            await _connection.Articles.InsertAsync(() => article);
        }

        public async Task AddKeywords(KeywordTable keyword)
        {
            await _connection.Keywords.InsertAsync(() => keyword);
        }
    }
}