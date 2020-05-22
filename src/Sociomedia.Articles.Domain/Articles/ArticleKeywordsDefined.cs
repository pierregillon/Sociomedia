using System;
using System.Collections.Generic;

namespace Sociomedia.Articles.Domain
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