using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Core;
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

        public async Task<IReadOnlyCollection<ArticleListItem>> List(int page, int pageSize, Guid? mediaId = null, string keyword = default)
        {
            var query = BuildArticlesQuery(mediaId, keyword);

            if (page > 0) {
                query = query.Skip(page * pageSize);
            }

            query = query.Take(pageSize);

            return await query.ToArrayAsync();
        }

        public async Task<int> Count(Guid? mediaId, string keyword)
        {
            return await BuildArticlesQuery(mediaId, keyword).CountAsync();
        }

        private IQueryable<ArticleListItem> BuildArticlesQuery(Guid? mediaId = null, string keyword = default)
        {
            var query =
                from article in _dbConnection.Articles
                join media in _dbConnection.Medias on article.MediaId equals media.Id
                orderby article.PublishDate descending
                select new {
                    Id = article.Id,
                    Title = article.Title,
                    Url = article.Url,
                    Summary = article.Summary,
                    ImageUrl = article.ImageUrl,
                    PublishDate = article.PublishDate,
                    MediaId = article.MediaId,
                    Keywords = article.Keywords,
                    MediaImageUrl = media.ImageUrl
                };

            if (!string.IsNullOrWhiteSpace(keyword)) {
                var keywords = keyword
                    .RemoveDiacritics()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToLower())
                    .ToArray();

                query = keywords.Aggregate(query, (current, k) => current.Where(x => x.Keywords.Contains(k)));
            }

            if (mediaId.HasValue) {
                query = query.Where(x => x.MediaId == mediaId.Value);
            }

            return query.Select(x => new ArticleListItem {
                Id = x.Id,
                Title = x.Title,
                Url = x.Url,
                Summary = x.Summary,
                ImageUrl = x.ImageUrl,
                PublishDate = x.PublishDate,
                MediaId = x.MediaId,
                MediaImageUrl = x.MediaImageUrl
            });
        }
    }
}