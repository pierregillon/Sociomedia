using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class ThemeFinder
    {
        private static readonly TimeSpan TWO_WEEKS = TimeSpan.FromDays(14);
        private const int MINIMUM_KEYWORD_COUNT = 1;
        private const int MINIMUM_ARTICLE_COUNT = 5;
        private const int MAXIMUM_ARTICLE_COUNT = 50;

        private readonly DbConnectionReadModel _dbConnection;

        public ThemeFinder(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IReadOnlyCollection<TrendingThemeListItem>> GetTrending()
        {
            var query =
                from theme in _dbConnection.Themes
                join themedArticle in _dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                join article in _dbConnection.Articles on themedArticle.ArticleId equals article.Id
                join media in _dbConnection.Medias on article.MediaId equals media.Id
                where article.PublishDate > DateTime.Now.Subtract(TWO_WEEKS)
                where theme.KeywordCount > MINIMUM_KEYWORD_COUNT
                select new {
                    ThemeId = theme.Id,
                    ThemeName = theme.Name,
                    ArticleId = article.Id,
                    ArticleTitle = article.Title,
                    article.ImageUrl,
                    article.Summary,
                    article.Url,
                    article.PublishDate,
                    article.MediaId,
                    MediaImageUrl = media.ImageUrl
                };

            var groupedQuery = query
                .GroupBy(x => new { x.ThemeId, x.ThemeName })
                .Where(x => x.Count().Between(MINIMUM_ARTICLE_COUNT, MAXIMUM_ARTICLE_COUNT))
                .OrderByDescending(x => x.Count());

            var results = await groupedQuery.ToArrayAsync();

            return results
                .Select(group => new TrendingThemeListItem {
                    Id = group.Key.ThemeId,
                    Name = group.Key.ThemeName,
                    Articles = group.Take(4).Select(article => new ArticleListItem {
                        Id = article.ArticleId,
                        Title = article.ArticleTitle,
                        ImageUrl = article.ImageUrl,
                        Summary = article.Summary,
                        Url = article.Url,
                        MediaId = article.MediaId,
                        MediaImageUrl = article.MediaImageUrl,
                        PublishDate = article.PublishDate
                    })
                }).ToArray();
        }

        public async Task<ThemeDetail> Details(Guid themeId)
        {
            var queryTheme =
                from theme in _dbConnection.Themes
                where theme.Id == themeId
                select new ThemeDetail {
                    Id = theme.Id,
                    Name = theme.Name,
                };

            return await queryTheme.SingleAsync();
        }
    }
}