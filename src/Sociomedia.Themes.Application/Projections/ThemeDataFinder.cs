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
        private readonly ThemeProjectionRepository _themeProjectionRepository;
        private readonly ThemeCalculatorConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IClock _clock;

        public ThemeDataFinder(ThemeProjectionRepository themeProjectionRepository, ThemeCalculatorConfiguration configuration, ILogger logger, IClock clock)
        {
            _themeProjectionRepository = themeProjectionRepository;
            _configuration = configuration;
            _logger = logger;
            _clock = clock;
        }

        public IReadOnlyCollection<ThemeReadModel> GetAllThemes()
        {
            return _themeProjectionRepository.GetAllThemes().ToArray();
        }

        public IReadOnlyCollection<ArticleReadModel> GetAllArticles(Guid articleId)
        {
            return _themeProjectionRepository.GetAllArticles()
                .Where(x => x.Id != articleId)
                .Where(x => x.Keywords.Any())
                .Where(x => x.PublishDate > _clock.Now().Subtract(_configuration.ArticleAggregationInterval))
                .ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetThemesWithAllKeywordsIncluded(Keywords intersection)
        {
            return _themeProjectionRepository.GetAllThemes()
                .Where(theme => theme.Keywords.ContainsAll(intersection))
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