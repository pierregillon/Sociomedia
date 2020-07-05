using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Tools;
using Sociomedia.Core.Domain;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class ThemeFinder
    {
        private static readonly TimeSpan TRENDING_INTERVAL = TimeSpan.FromDays(7);
        private static readonly TimeSpan ONE_MONTH = TimeSpan.FromDays(30);
        private const int MINIMUM_KEYWORD_COUNT = 1;
        private const int MINIMUM_ARTICLE_COUNT = 5;
        private const int MONTH_TRENDING_THEME_COUNT = 7;
        private const int MAX_SCORE = 10;
        private const int KEYWORD_COUNT_FOR_MAX_SCORE = 5;
        private const int KEYWORD_DENSITY_FOR_MAX_SCORE = 7;
        private const int MINIMUM_KEYWORD_DENSITY = 3;
        private const int ARTICLE_COUNT_FOR_MAX_SCORE = 8;

        public async Task<IReadOnlyCollection<ThemeListItem>> GetMonthTrending()
        {
            await using var dbConnection = new DbConnectionReadModel();

            var now = DateTimeOffset.Now;

            var invalidWords = new[] {
                "janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre",
                "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi", "dimanche",
                "france"
            };

            return await (from theme in dbConnection.Themes
                    join themedArticle in dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                    join article in dbConnection.Articles on themedArticle.ArticleId equals article.Id
                    where article.PublishDate > now.Subtract(ONE_MONTH)
                    where !theme.Name.ToLower().In(invalidWords)
                    orderby article.PublishDate descending
                    select new {
                        ThemeId = theme.Id,
                        ThemeName = theme.Name
                    })
                .GroupBy(x => new { x.ThemeId, x.ThemeName })
                .OrderByDescending(x => x.Count())
                .Take(MONTH_TRENDING_THEME_COUNT)
                .Select(x => new ThemeListItem {
                    Id = x.Key.ThemeId,
                    Name = x.Key.ThemeName
                })
                .ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<TrendingThemeListItem>> GetTrending(int page, int pageSize)
        {
            await using var dbConnection = new DbConnectionReadModel();

            var now = DateTimeOffset.Now;

            var betterThemes = await GetBestThemes(dbConnection, now);

            var themeIds = betterThemes
                .Select(x => x.ThemeId)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToArray();

            if (!themeIds.Any()) {
                return Array.Empty<TrendingThemeListItem>();
            }

            var results = await (
                    from theme in dbConnection.Themes
                    join themedArticle in dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                    join article in dbConnection.Articles on themedArticle.ArticleId equals article.Id
                    join media in dbConnection.Medias on article.MediaId equals media.Id
                    where article.PublishDate > now.Subtract(TRENDING_INTERVAL)
                    where themeIds.Contains(theme.Id)
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
                    })
                .GroupBy(x => new { x.ThemeId, x.ThemeName })
                .ToArrayAsync();

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
                .Pipe(x => RestoreOrder(x.ToArray(), themeIds))
                .Select(group => group.BuildTrending())
                .ToArray();
        }

        private static async Task<ThemeWithBetterScore[]> GetBestThemes(DbConnectionReadModel dbConnection, DateTimeOffset now)
        {
            var articles =
                from article in dbConnection.Articles
                where article.PublishDate > now.Subtract(TRENDING_INTERVAL)
                select new {
                    article.Id,
                    UpToDateScore = ((decimal) TRENDING_INTERVAL.TotalDays - SqlExtensions.DayCountBetween(now, article.PublishDate)) * MAX_SCORE / (int) TRENDING_INTERVAL.TotalDays
                };

            var betterThemes = await (
                    from theme in dbConnection.Themes
                    join themedArticle in dbConnection.ThemedArticles on theme.Id equals themedArticle.ThemeId
                    join article in articles on themedArticle.ArticleId equals article.Id
                    where theme.KeywordCount >= MINIMUM_KEYWORD_COUNT
                    where theme.OccurencePerKeywordPerArticle > MINIMUM_KEYWORD_DENSITY
                    select new {
                        ThemeId = theme.Id,
                        ThemeFullKeywords = theme.FullKeywords,
                        ThemeKeywordCountScore = (decimal) Math.Min(theme.KeywordCount, KEYWORD_COUNT_FOR_MAX_SCORE) * MAX_SCORE / KEYWORD_COUNT_FOR_MAX_SCORE,
                        ThemeOccurencePerKeywordPerArticleScore = (decimal) Math.Min(theme.OccurencePerKeywordPerArticle, KEYWORD_DENSITY_FOR_MAX_SCORE) * MAX_SCORE / KEYWORD_DENSITY_FOR_MAX_SCORE,
                        article.Id,
                        article.UpToDateScore
                    })
                .GroupBy(x => new { x.ThemeId, x.ThemeFullKeywords, x.ThemeKeywordCountScore, x.ThemeOccurencePerKeywordPerArticleScore })
                .Where(x => x.Count() > MINIMUM_ARTICLE_COUNT)
                .OrderByDescending(x =>
                    x.Average(y => y.UpToDateScore)
                    + x.Key.ThemeKeywordCountScore
                    + x.Key.ThemeOccurencePerKeywordPerArticleScore
                    + (decimal) Math.Min(x.Count(), ARTICLE_COUNT_FOR_MAX_SCORE) * MAX_SCORE / ARTICLE_COUNT_FOR_MAX_SCORE
                )
                .Select(x => new ThemeWithBetterScore(x.Key.ThemeId, x.Key.ThemeFullKeywords))
                .ToArrayAsync();

            return FilterThemesWithCloseContent(betterThemes).ToArray();
        }

        private static IEnumerable<ThemeWithBetterScore> FilterThemesWithCloseContent(IReadOnlyCollection<ThemeWithBetterScore> betterThemes)
        {
            var processedThemes = new List<ThemeWithBetterScore>();
            foreach (var theme in betterThemes) {
                if (processedThemes.Any(x => x.DealsWithTheSameSubject(theme))) {
                    continue;
                }
                processedThemes.Add(theme);
                yield return theme;
            }
        }

        private static IEnumerable<ThemeGroup> RestoreOrder(IReadOnlyCollection<ThemeGroup> results, IEnumerable<Guid> orderedThemeIds)
        {
            return orderedThemeIds
                .Select(themeId => results.SingleOrDefault(group => group.ThemeId == themeId))
                .Where(result => result != null);
        }

        public async Task<ThemeDetail> Details(Guid themeId)
        {
            await using var dbConnection = new DbConnectionReadModel();

            var queryTheme =
                from theme in dbConnection.Themes
                where theme.Id == themeId
                select new ThemeDetail {
                    Id = theme.Id,
                    Name = theme.Name,
                };

            return await queryTheme.SingleAsync();
        }
    }

    public class ThemeWithBetterScore
    {
        private readonly IReadOnlyCollection<string> _keywords2;

        public Guid ThemeId { get; }
        public string Keywords { get; }

        public ThemeWithBetterScore(Guid themeId, string keywords)
        {
            ThemeId = themeId;
            Keywords = keywords;

            _keywords2 = Regex
                .Replace(keywords, @"(\(\d*\))", string.Empty)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }

        public bool DealsWithTheSameSubject(ThemeWithBetterScore theme)
        {
            return theme._keywords2.Intersect(_keywords2).Count() >= 1;
        }
    }

    public class ThemeListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ThemeGroup
    {
        public Guid ThemeId { get; }
        private readonly string _themeName;
        private readonly IReadOnlyCollection<ArticleListItem> _articles;

        public ThemeGroup(Guid themeId, string themeName, IReadOnlyCollection<ArticleListItem> articles)
        {
            ThemeId = themeId;
            _themeName = themeName;
            _articles = articles;
        }


        public TrendingThemeListItem BuildTrending()
        {
            return new TrendingThemeListItem {
                Id = ThemeId,
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