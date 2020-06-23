using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ArticleReadModel
    {
        private IReadOnlyCollection<Keyword> _keywordsAndOccurence;
        public Guid Id { get;  }
        public DateTimeOffset PublishDate { get; }
        public IReadOnlyCollection<string> Keywords { get; private set; }

        public ArticleReadModel(Guid id, DateTimeOffset publishDate)
        {
            Id = id;
            PublishDate = publishDate;
        }

        public void DefineKeywords(IReadOnlyCollection<Keyword> keywords)
        {
            Keywords = keywords.Select(x => x.Value).ToArray();
            _keywordsAndOccurence = keywords ?? throw new ArgumentNullException(nameof(keywords));
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