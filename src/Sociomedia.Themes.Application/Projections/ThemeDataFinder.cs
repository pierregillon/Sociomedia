using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeDataFinder
    {
        private readonly ThemeProjection _themeProjection;
        private readonly TimeSpan _articleAggregationInterval;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public ThemeDataFinder(ThemeProjection themeProjection, TimeSpan articleAggregationInterval, IClock clock, ILogger logger)
        {
            _themeProjection = themeProjection;
            _articleAggregationInterval = articleAggregationInterval;
            _clock = clock;
            _logger = logger;
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesWithRecentlyArticleAdded()
        {
            return _themeProjection.Themes
                .Select(x => x.FilterRecentArticlesFrom(_clock.Now().Subtract(_articleAggregationInterval)))
                .Where(x => x.Articles.Any())
                .ToArray();
        }

        public IReadOnlyCollection<ArticleReadModel> GetRecentArticles()
        {
            return _themeProjection.Articles
                .Where(x => x.PublishDate > _clock.Now().Subtract(_articleAggregationInterval))
                .ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetAllKeywordIncludedThemes(KeywordIntersection intersection)
        {
            return GetThemesWithRecentlyArticleAdded()
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
    }
}