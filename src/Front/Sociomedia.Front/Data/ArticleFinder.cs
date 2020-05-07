using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class ArticleFinder
    {
        private readonly DbConnectionReadModel _dbConnection;

        public ArticleFinder(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IReadOnlyCollection<ArticleListItem>> List()
        {
            var query = from article in _dbConnection.Articles
                join media in _dbConnection.Medias on article.MediaId equals media.Id
                orderby article.PublishDate descending
                select new ArticleListItem {
                    Id = article.Id,
                    Title = article.Title,
                    Url = article.Url,
                    Summary = article.Summary,
                    ImageUrl = article.ImageUrl,
                    PublishDate = article.PublishDate,
                    MediaImageUrl = media.ImageUrl
                };

            return await query.ToArrayAsync();
        }
    }
}