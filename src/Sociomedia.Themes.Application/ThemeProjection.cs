using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeProjection
    {
        private readonly List<ArticleReadModel> _articles = new List<ArticleReadModel>();
        private readonly List<ThemeReadModel> _themes = new List<ThemeReadModel>();

        public IReadOnlyCollection<ArticleReadModel> Articles => _articles;
        public IReadOnlyCollection<ThemeReadModel> Themes => _themes;

        public void AddArticle(ArticleKeywordsDefined @event)
        {
            var article = _articles.SingleOrDefault(x => x.Id == @event.Id);
            if (article == null) {
                article = new ArticleReadModel(@event.Id, @event.Keywords.Select(x => new Keyword2(x.Value, x.Occurence)).ToArray());
                _articles.Add(article);
            }
            else {
                article.Keywords = @event.Keywords.Select(x => new Keyword2(x.Value, x.Occurence)).ToArray();
            }
        }

        public void AddTheme(ThemeAdded @event)
        {
            var articles = _articles.Where(x => @event.Articles.Contains(x.Id)).ToArray();

            var theme = _themes.SingleOrDefault(x => x.Id == @event.Id);
            if (theme == null) {
                theme = new ThemeReadModel(@event.Id, @event.Keywords.Select(x => new Keyword2(x.Value, x.Occurence)).ToArray(), articles);
                _themes.Add(theme);
            }
            else {
                throw new InvalidOperationException("theme already added !");
            }
        }

        public void AddArticleToTheme(ArticleAddedToTheme @event)
        {
            var article = _articles.Single(x => x.Id == @event.ArticleId);
            var theme = _themes.Single(x => x.Id == @event.Id);
            theme.AddArticle(article);
        }
    }
}