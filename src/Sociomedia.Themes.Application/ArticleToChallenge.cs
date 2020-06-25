using System;
using System.Collections.Generic;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ArticleToChallenge
    {
        public Guid Id { get; }
        public DateTimeOffset PublishDate { get; }
        public IReadOnlyCollection<Keyword> Keywords { get; }

        public ArticleToChallenge(Guid id, DateTimeOffset publishDate, IReadOnlyCollection<Keyword> keywords)
        {
            Id = id;
            PublishDate = publishDate;
            Keywords = keywords;
        }

        public Article ToDomain()
        {
            return new Article(Id, Keywords);
        }
    }
}