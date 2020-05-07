using System;

namespace Sociomedia.DomainEvents.Article
{
    public abstract class ArticleEvent : DomainEvent
    {
        protected ArticleEvent(Guid id) : base(id, "article") { }
    }
}