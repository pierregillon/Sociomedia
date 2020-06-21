using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Tools;
using Sociomedia.Core;
using Sociomedia.Front.Models;
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

        public async Task<IReadOnlyCollection<ArticleListItem>> List(int page, int pageSize, ArticleListFilters filters)
        {
            var query = BuildArticlesQuery(filters);

            if (page > 0) {
                query = query.Skip(page * pageSize);
            }

            query = query.Take(pageSize);

            return await query.ToArrayAsync();
        }

        public async Task<int> Count(ArticleListFilters filters)
        {
            return await BuildArticlesQuery(filters).CountAsync();
        }

        private IQueryable<ArticleListItem> BuildArticlesQuery(ArticleListFilters filters)
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

            if (!string.IsNullOrWhiteSpace(filters.Search)) {
                var keywords = filters.Search
                    .RemoveDiacritics()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToLower())
                    .ToArray();

                query = keywords.Aggregate(query, (current, k) => current.Where(x => x.Keywords.Contains(k)));
            }

            if (filters.MediaId.HasValue) {
                query = query.Where(x => x.MediaId == filters.MediaId.Value);
            }

            if (filters.ThemeId.HasValue) {
                var inThemeSubQuery =
                    from theme in _dbConnection.Themes
                    join themedArticle in _dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                    where theme.Id == filters.ThemeId.Value
                    select themedArticle.ArticleId;

                query = query.Where(x => x.Id.In(inThemeSubQuery));
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