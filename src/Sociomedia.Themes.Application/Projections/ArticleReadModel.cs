using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ArticleReadModel
    {
        public Guid Id { get; set; }
        public IReadOnlyCollection<Keyword2> Keywords { get; set; }

        public ArticleReadModel(Guid id, IReadOnlyCollection<Keyword2> keywords)
        {
            Id = id;
            Keywords = keywords;
        }

        public KeywordIntersection CommonKeywords(Article article)
        {
            return new KeywordIntersection(Keywords
                .Join(article.Keywords, x => x.Value, y => y.Value, (x, y) => x + y)
                .ToArray()
                .Pipe(x => x));
        }

        public bool ContainsKeywords(IReadOnlyCollection<Keyword2> keywords)
        {
            return keywords.All(Keywords.Contains);
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords.Select(x => x.ToString()).ToArray());
        }
    }
}