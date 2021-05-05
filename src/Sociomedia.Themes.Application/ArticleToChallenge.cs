using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ArticleToChallenge
    {
        private readonly IReadOnlyCollection<Keyword> _keywordsWithOccurence;

        public Guid Id { get; }
        public DateTimeOffset PublishDate { get; }
        public Keywords Keywords { get; }

        public ArticleToChallenge(Guid id, DateTimeOffset publishDate, IReadOnlyCollection<Keyword> keywordsWithOccurence)
        {
            _keywordsWithOccurence = keywordsWithOccurence;
            Id = id;
            PublishDate = publishDate;
            Keywords = new Keywords(keywordsWithOccurence.Select(x=>x.Value).ToArray());
        }

        public Article ToDomain()
        {
            return new Article(Id, _keywordsWithOccurence);
        }
    }
}