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

        public IReadOnlyCollection<ThemeReadModel> GetThemesContainingArticlesInSameTimeFrame(ArticleToChallenge article)
        {
            return _themeProjection.Themes
                .Select(x => x.FilterRecentArticlesFrom(article.PublishDate.Subtract(_articleAggregationInterval)))
                .Where(x => x.Articles.Any())
                .ToArray();
        }

        public IReadOnlyCollection<ArticleReadModel> GetArticlesInSameTimeFrame(ArticleToChallenge article)
        {
            return _themeProjection.Articles
                .Where(x => x.Id != article.Id)
                .Where(x=>x.Keywords.Any())
                .Where(x => x.PublishDate > article.PublishDate.Subtract(_articleAggregationInterval))
                .ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesWithAllKeywordsIncluded(Keywords intersection, ArticleToChallenge article)
        {
            return GetThemesContainingArticlesInSameTimeFrame(article)
                .Where(theme => intersection.ContainsAll(theme.Keywords))
                .ToList();
        }

        public ThemeReadModel FindExistingTheme(Keywords intersection)
        {
            var themes = _themeProjection.Themes.Where(x => intersection.SequenceEqual(x.Keywords)).ToArray();
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