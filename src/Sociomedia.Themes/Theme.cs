using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace Sociomedia.Themes.Domain
{
    public class Theme : AggregateRoot
    {
        private Theme() { }

        public Theme(IEnumerable<ThemeEvent> events)
        {
            ApplyChanges(events);
        }

        public Theme(IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles)
        {
            ApplyChange(new ThemeAdded(Guid.NewGuid(), keywords, articles));
        }

        public void AddArticle(Article article)
        {
            ApplyChange(new ArticleAddedToTheme(Id, article.Id));
        }

        private void Apply(ThemeAdded @event)
        {
            Id = @event.Id;
        }

        public void ApplyChanges(IEnumerable<ThemeEvent> events)
        {
            foreach (var @event in @events) {
                ApplyChange(@event);
            }
        }
    }
}