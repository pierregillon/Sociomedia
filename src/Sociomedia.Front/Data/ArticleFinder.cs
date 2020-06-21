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
        public async Task<IReadOnlyCollection<ArticleListItem>> List(int page, int pageSize, ArticleListFilters filters)
        {
            await using var dbConnection = new DbConnectionReadModel();

            var query = BuildArticlesQuery(dbConnection, filters);

            if (page > 0) {
                query = query.Skip(page * pageSize);
            }

            query = query.Take(pageSize);

            return await query.ToArrayAsync();
        }

        public async Task<int> Count(ArticleListFilters filters)
        {
            await using var dbConnection = new DbConnectionReadModel();
            
            return await BuildArticlesQuery(dbConnection, filters).CountAsync();
        }

        private static IQueryable<ArticleListItem> BuildArticlesQuery(DbConnectionReadModel dbConnection, ArticleListFilters filters)
        {
            var query =
                from article in dbConnection.Articles
                join media in dbConnection.Medias on article.MediaId equals media.Id
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
                    from theme in dbConnection.Themes
                    join themedArticle in dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
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