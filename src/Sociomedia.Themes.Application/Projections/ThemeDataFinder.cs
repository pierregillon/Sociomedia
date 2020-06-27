using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeDataFinder
    {
        private readonly ThemeProjectionRepository _themeProjectionRepository;
        private readonly TimeSpan _articleAggregationInterval;
        private readonly ILogger _logger;

        public ThemeDataFinder(ThemeProjectionRepository themeProjectionRepository, TimeSpan articleAggregationInterval, ILogger logger)
        {
            _themeProjectionRepository = themeProjectionRepository;
            _articleAggregationInterval = articleAggregationInterval;
            _logger = logger;
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesContainingArticlesInSameTimeFrame(ArticleToChallenge article)
        {
            return _themeProjectionRepository.GetAllThemes()
                .Select(x => x.FilterRecentArticlesFrom(article.PublishDate.Subtract(_articleAggregationInterval)))
                .Where(x => x.Articles.Any())
                .ToArray();
        }

        public IReadOnlyCollection<ArticleReadModel> GetArticlesInSameTimeFrame(ArticleToChallenge article)
        {
            return _themeProjectionRepository.GetAllArticles()
                .Where(x => x.Id != article.Id)
                .Where(x => x.Keywords.Any())
                .Where(x => x.PublishDate > article.PublishDate.Subtract(_articleAggregationInterval))
                .ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesWithAllKeywordsIncluded(Keywords intersection, ArticleToChallenge article)
        {
            return _themeProjectionRepository.GetAllThemes()
                .Where(theme => theme.Keywords.ContainsAll(intersection))
                .Select(x => x.FilterRecentArticlesFrom(article.PublishDate.Subtract(_articleAggregationInterval)))
                .Where(x => x.Articles.Any())
                .ToList();
        }

        public ThemeReadModel FindExistingTheme(Keywords intersection)
        {
            var themes = _themeProjectionRepository.GetAllThemes().Where(x => intersection.SequenceEqual(x.Keywords)).ToArray();
            if (themes.Length > 1) {
                _logger.Info("2 themes have the same keyword intersections !");
            }
            return themes.FirstOrDefault();
        }

        public ArticleReadModel GetArticle(Guid articleId)
        {
            return _themeProjectionRepository.FindArticle(articleId);
        }
    }
}