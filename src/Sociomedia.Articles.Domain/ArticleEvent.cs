using System;
using Sociomedia.Application.Domain;

namespace Sociomedia.Articles.Domain
{
    public abstract class ArticleEvent : DomainEvent
    {
        protected ArticleEvent(Guid id) : base(id, "article") { }
    }
}