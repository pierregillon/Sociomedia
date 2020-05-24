using System;
using System.Collections.Generic;
using Sociomedia.Articles.Domain.Keywords;

namespace Sociomedia.Articles.Domain.Articles
{
    public class ArticleKeywordsDefined : ArticleEvent
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }

        public ArticleKeywordsDefined(Guid id, IReadOnlyCollection<Keyword> keywords) : base(id)
        {
            Keywords = keywords;
        }
    }
}