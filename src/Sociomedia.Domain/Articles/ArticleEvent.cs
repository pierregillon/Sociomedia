using System;

namespace Sociomedia.Domain.Articles
{
    public abstract class ArticleEvent : DomainEvent
    {
        protected ArticleEvent(Guid id) : base(id, "article") { }
    }
}