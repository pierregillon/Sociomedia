using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Application.Projections;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeProjection :
        IEventListener<ArticleKeywordsDefined>,
        IEventListener<ThemeAdded>,
        IEventListener<ArticleAddedToTheme>,
        IProjection
    {
        private readonly InMemoryDatabase _inMemoryDatabase;

        public ThemeProjection(InMemoryDatabase inMemoryDatabase)
        {
            _inMemoryDatabase = inMemoryDatabase;
        }

        public IReadOnlyCollection<ArticleReadModel> Articles => _inMemoryDatabase.List<ArticleReadModel>();
        public IReadOnlyCollection<ThemeReadModel> Themes => _inMemoryDatabase.List<ThemeReadModel>();

        public Task On(ArticleKeywordsDefined @event)
        {
            var article = Articles.SingleOrDefault(x => x.Id == @event.Id);
            if (article == null) {
                _inMemoryDatabase.Add(new ArticleReadModel(@event.Id, @event.Keywords.Select(x => new Keyword(x.Value, x.Occurence)).ToArray()));
            }
            else {
                _inMemoryDatabase.Remove(article);
                _inMemoryDatabase.Add(new ArticleReadModel(@event.Id, @event.Keywords.Select(x => new Keyword(x.Value, x.Occurence)).ToArray()));
            }
            return Task.CompletedTask;
        }

        public Task On(ThemeAdded @event)
        {
            var articles = Articles.Where(x => @event.Articles.Contains(x.Id)).ToArray();

            var theme = Themes.SingleOrDefault(x => x.Id == @event.Id);
            if (theme == null) {
                theme = new ThemeReadModel(@event.Id, @event.Keywords.Select(x => x.Value).ToArray(), articles);
                _inMemoryDatabase.Add(theme);
            }
            else {
                throw new InvalidOperationException("theme already added !");
            }
            return Task.CompletedTask;
        }

        public Task On(ArticleAddedToTheme @event)
        {
            var article = Articles.Single(x => x.Id == @event.ArticleId);
            var theme = Themes.Single(x => x.Id == @event.Id);
            theme.AddArticle(article);
            return Task.CompletedTask;
        }
    }
}