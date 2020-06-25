using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeDataFinder
    {
        private readonly ThemeProjection _themeProjection;
        private readonly TimeSpan _articleAggregationInterval;
        private readonly ILogger _logger;

        public ThemeDataFinder(ThemeProjection themeProjection, TimeSpan articleAggregationInterval, ILogger logger)
        {
            _themeProjection = themeProjection;
            _articleAggregationInterval = articleAggregationInterval;
            _logger = logger;
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesContainingArticlesInSameTimeFrame(DateTimeOffset date)
        {
            return _themeProjection.Themes
                .Select(x => x.FilterRecentArticlesFrom(date.Subtract(_articleAggregationInterval)))
                .Where(x => x.Articles.Any())
                .ToArray();
        }

        public IReadOnlyCollection<ArticleReadModel> GetArticlesInSameTimeFrame(DateTimeOffset date)
        {
            return _themeProjection.Articles
                .Where(x => x.PublishDate > date.Subtract(_articleAggregationInterval))
                .ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesWithAllKeywordsIncluded(KeywordIntersection intersection, DateTimeOffset date)
        {
            return GetThemesContainingArticlesInSameTimeFrame(date)
                .Where(theme => intersection.ContainsAllWords(theme.Keywords))
                .ToList();
        }

        public ThemeReadModel FindTheme(KeywordIntersection intersection)
        {
            var themes = _themeProjection.Themes.Where(x => intersection.SequenceEquals(x.Keywords)).ToArray();
            if (themes.Length > 1) {
                _logger.Info("2 themes have the same keyword intersections !");
            }
            return themes.FirstOrDefault();
        }

        public ArticleReadModel GetArticle(Guid articleId)
        {
            return _themeProjection.Articles.FirstOrDefault(x => x.Id == articleId);
        }
    }
}