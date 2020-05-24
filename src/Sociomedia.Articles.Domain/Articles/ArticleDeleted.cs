using System;

namespace Sociomedia.Articles.Domain.Articles {
    public class ArticleDeleted : ArticleEvent
    {
        public ArticleDeleted(Guid id):base(id)
        {
        }
    }
}