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

        public Keywords2 CommonKeywords(Article article)
        {
            return new Keywords2(Keywords
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

    public class Keywords2
    {
        private readonly IReadOnlyCollection<Keyword2> _keywords;

        public Keywords2(IReadOnlyCollection<Keyword2> keywords)
        {
            _keywords = keywords;
        }

        public int Count => _keywords.Count;

        public bool Any()
        {
            return _keywords.Any();
        }

        public bool ContainsAllWords(IReadOnlyCollection<Keyword2> keywords)
        {
            return keywords.All(keyword => _keywords.Select(x => x.Value).Any(y => keyword.Value == y));
        }

        public IReadOnlyCollection<Keyword2> ToArray()
        {
            return _keywords;
        }

        public bool SequenceEquals(IReadOnlyCollection<Keyword2> keywords)
        {
            return _keywords.Select(x => x.Value).Intersect(keywords.Select(x => x.Value)).Count() == keywords.Count;
        }

        public override string ToString()
        {
            return string.Join(" | ", _keywords.Select(x => x.ToString()).ToArray());
        }
    }
}