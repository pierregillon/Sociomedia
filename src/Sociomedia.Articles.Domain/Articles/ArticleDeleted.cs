using System;

namespace Sociomedia.Articles.Domain {
    public class ArticleDeleted : ArticleEvent
    {
        public ArticleDeleted(Guid id):base(id)
        {
        }
    }
}