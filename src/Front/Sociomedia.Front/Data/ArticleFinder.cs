using System;
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

        public async Task<IReadOnlyCollection<ArticleListItem>> List(int page, int pageSize, Guid? mediaId = null)
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
                    MediaId = article.MediaId,
                    MediaImageUrl = media.ImageUrl
                };

            if (mediaId.HasValue) {
                query = query.Where(x => x.MediaId == mediaId.Value);
            }

            if (page > 0) {
                query = query.Skip(page * pageSize);
            }

            query = query.Take(pageSize);

            return await query.ToArrayAsync();
        }
    }
}