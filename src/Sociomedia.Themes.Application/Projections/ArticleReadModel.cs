using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ArticleReadModel
    {
        private readonly IReadOnlyCollection<Keyword> _keywordsAndOccurence;
        public Guid Id { get; set; }
        public IReadOnlyCollection<string> Keywords { get; }

        public ArticleReadModel(Guid id, IReadOnlyCollection<Keyword> keywords)
        {
            Id = id;
            Keywords = keywords.Select(x => x.Value).ToArray();
            _keywordsAndOccurence = keywords;
        }

        public KeywordIntersection CommonKeywords(Article article)
        {
            return new KeywordIntersection(Keywords
                .Intersect(article.Keywords.Select(x => x.Value))
                .ToArray()
                .Pipe(x => x));
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords.Select(x => x.ToString()).ToArray());
        }

        public Article ToDomain()
        {
            return new Article(Id, _keywordsAndOccurence);
        }
    }
}