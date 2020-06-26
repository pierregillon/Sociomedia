using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ArticleReadModel
    {
        private IReadOnlyCollection<Keyword> _keywordsAndOccurence;

        public Guid Id { get; }
        public DateTimeOffset PublishDate { get; }
        public Keywords Keywords { get; private set; }

        public ArticleReadModel(Guid id, DateTimeOffset publishDate)
        {
            Id = id;
            PublishDate = publishDate;
            Keywords = new Keywords(Array.Empty<string>());
        }

        public void DefineKeywords(IReadOnlyCollection<Keyword> keywords)
        {
            _keywordsAndOccurence = keywords;
            Keywords = new Keywords(keywords.Select(x => x.Value).ToArray());
        }

        public Keywords IntersectKeywords(ArticleToChallenge article)
        {
            return Keywords.Intersect(article.Keywords);
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords.ToValues().Select(x => x.ToString()).ToArray());
        }

        public Article ToDomain()
        {
            return new Article(Id, _keywordsAndOccurence);
        }
    }
}