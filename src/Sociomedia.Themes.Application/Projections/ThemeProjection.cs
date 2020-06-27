using System;
using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeProjection :
        IEventListener<ArticleImported>,
        IEventListener<ArticleKeywordsDefined>,
        IEventListener<ThemeAdded>,
        IEventListener<ArticleAddedToTheme>,
        IProjection
    {
        private readonly ThemeProjectionRepository _themeProjectionRepository;

        public ThemeProjection(ThemeProjectionRepository themeProjectionRepository)
        {
            _themeProjectionRepository = themeProjectionRepository;
        }

        // ----- Callbacks

        public Task On(ArticleImported @event)
        {
            var existingArticle = _themeProjectionRepository.FindArticle(@event.Id);
            if (existingArticle != null) {
                throw new InvalidOperationException($"The article {@event.Id} already exists in projection !");
            }

            return _themeProjectionRepository.AddArticle(new ArticleReadModel(@event.Id, @event.PublishDate));
        }

        public Task On(ArticleKeywordsDefined @event)
        {
            var article = _themeProjectionRepository.FindArticle(@event.Id);
            if (article == null) {
                throw new InvalidOperationException($"The article {@event.Id} does not exists in projection : unable to define keywords !");
            }
            article.DefineKeywords(@event.Keywords.Select(x => new Keyword(x.Value, x.Occurence)).ToArray());
            return Task.CompletedTask;
        }

        public Task On(ThemeAdded @event)
        {
            var theme = _themeProjectionRepository.FindTheme(@event.Id);
            if (theme != null) {
                throw new InvalidOperationException($"The theme {@event.Id} already exists in projection.");
            }
            return _themeProjectionRepository.AddTheme(new ThemeReadModel(@event.Id, @event.Keywords.Select(x => x.Value).ToArray(), _themeProjectionRepository.FindArticles(@event.Articles)));
        }

        public Task On(ArticleAddedToTheme @event)
        {
            var article = _themeProjectionRepository.FindArticle(@event.ArticleId);
            if (article == null) {
                throw new InvalidOperationException($"The article {@event.ArticleId} does not exists in projection : unable to add article to theme {@event.Id}.");
            }
            var theme = _themeProjectionRepository.FindTheme(@event.Id);
            if (theme == null) {
                throw new InvalidOperationException($"The article {@event.ArticleId} does not exists in projection : unable to add article to theme {@event.Id}.");
            }
            theme.AddArticle(article);
            return Task.CompletedTask;
        }
    }
}