using System;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Domain
{
    public abstract class ArticleEvent : DomainEvent
    {
        protected ArticleEvent(Guid id) : base(id, "article") { }
    }
}