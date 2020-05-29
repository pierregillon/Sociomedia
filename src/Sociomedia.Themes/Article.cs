using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class Article
    {
        public Guid Id { get; }
        public IReadOnlyCollection<Keyword2> Keywords { get; }

        public Article(Guid id, IReadOnlyCollection<Keyword2> keywords)
        {
            Id = id;
            Keywords = keywords;
        }

        public IReadOnlyCollection<Keyword2> CommonKeywords(Article article)
        {
            return Keywords.Join(article.Keywords, x => x.Value, y => y.Value, (x, y) => x + y).ToArray();
        }

        public bool ContainsKeywords(IReadOnlyCollection<Keyword2> keywords)
        {
            return keywords.All(Keywords.Contains);
        }
    }
}