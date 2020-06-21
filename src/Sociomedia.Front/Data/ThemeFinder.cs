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
        private const int THEME_COUNT = 10;

        private readonly DbConnectionReadModel _dbConnection;

        public ThemeFinder(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IReadOnlyCollection<TrendingThemeListItem>> GetTrending(int page, int pageSize)
        {
            var now = DateTimeOffset.Now;

            var query =
                (from theme in _dbConnection.Themes
                    join themedArticle in _dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                    join article in _dbConnection.Articles on themedArticle.ArticleId equals article.Id
                    join media in _dbConnection.Medias on article.MediaId equals media.Id
                    where article.PublishDate > now.Subtract(TWO_WEEKS)
                    where theme.KeywordCount > MINIMUM_KEYWORD_COUNT
                    orderby article.PublishDate descending
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
                        MediaImageUrl = media.ImageUrl,
                        Score = (int) TWO_WEEKS.TotalDays - SqlExtensions.DayCountBetween(now, article.PublishDate)
                    })
                .GroupBy(x => new { x.ThemeId, x.ThemeName })
                .Where(x => x.Count().Between(MINIMUM_ARTICLE_COUNT, MAXIMUM_ARTICLE_COUNT))
                .OrderByDescending(x => x.Sum(y => y.Score))
                .Skip(page * pageSize)
                .Take(pageSize);

            var results = await query.ToArrayAsync();

            return results
                .Select(x => new ThemeGroup(x.Key.ThemeId, x.Key.ThemeName, x.Select(article => new ArticleListItem {
                    Id = article.ArticleId,
                    Title = article.ArticleTitle,
                    ImageUrl = article.ImageUrl,
                    Summary = article.Summary,
                    Url = article.Url,
                    MediaId = article.MediaId,
                    MediaImageUrl = article.MediaImageUrl,
                    PublishDate = article.PublishDate
                }).ToArray()))
                .Select(group => group.BuildTrending())
                .ToArray();
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

    public class ThemeGroup
    {
        private readonly Guid _themeId;
        private readonly string _themeName;
        private readonly IReadOnlyCollection<ArticleListItem> _articles;

        public ThemeGroup(Guid themeId, string themeName, IReadOnlyCollection<ArticleListItem> articles)
        {
            _themeId = themeId;
            _themeName = themeName;
            _articles = articles;
        }

        public TrendingThemeListItem BuildTrending()
        {
            return new TrendingThemeListItem {
                Id = _themeId,
                Name = _themeName,
                Articles = DistributeArticlesForEachMedia().Take(4).OrderByDescending(x => x.PublishDate).ToArray()
            };
        }

        private IEnumerable<ArticleListItem> DistributeArticlesForEachMedia()
        {
            var articlesGroupedByMedia = _articles
                .GroupBy(x => x.MediaId)
                .ToArray();

            for (var i = 0; i < articlesGroupedByMedia.Select(x => x.Count()).Max(x => x); i++) {
                foreach (var mediaArticles in articlesGroupedByMedia) {
                    var article = mediaArticles.Skip(i).Take(1).FirstOrDefault();
                    if (article != null) {
                        yield return article;
                    }
                }
            }
        }
    }

    public static class SqlExtensions
    {
        [Sql.Extension("Date_part('day', {from} - {to})", ServerSideOnly = true, PreferServerSide = false)]
        public static int DayCountBetween([ExprParameter] DateTimeOffset from, [ExprParameter] DateTimeOffset to)
        {
            throw new InvalidOperationException("dqfs");
        }
    }
}