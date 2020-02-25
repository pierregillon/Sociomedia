using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.Themes
{
    public class ThemeManager
    {
        private readonly List<ThemeArticle> _articles = new List<ThemeArticle>();
        private readonly List<Theme> _themes = new List<Theme>();
        private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents;

        public void Add(ThemeArticle article)
        {
            foreach (var existingArticle in _articles) {
                var keywordIntersection = existingArticle.Keywords.Intersect(article.Keywords).ToArray();
                if (!keywordIntersection.Any()) {
                    continue;
                }
                var existingTheme = _themes.FirstOrDefault(theme => keywordIntersection.All(keyword => theme.Keywords.Contains(keyword)));
                if (existingTheme == null) {
                    Apply(new NewThemeCreated(Guid.NewGuid(), keywordIntersection, new[] { existingArticle, article }));
                }
                else if (!existingTheme.Contains(article)) {
                    Apply(new ArticleAddedToTheme(existingTheme.Id, article));
                }
            }
            _articles.Add(article);
        }

        private void Apply<T>(T domainEvent) where T : IDomainEvent
        {
            ((dynamic) this).On(domainEvent);
            _uncommittedEvents.Add(domainEvent);
        }

        private void On(NewThemeCreated themeCreated)
        {
            _themes.Add(new Theme(themeCreated.Id, themeCreated.Keywords, themeCreated.Articles));
        }

        private void On(ArticleAddedToTheme @event)
        {
            var theme = _themes.Single(x => x.Id == @event.ThemeId);
            theme.AddArticle(@event.Article);
        }
    }
}