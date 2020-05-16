using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.Articles.Application.Queries
{
    public interface ISynchronizationFinder
    {
        Task<IReadOnlyCollection<MediaFeedReadModel>> GetAllMediaFeeds();
        Task<ArticleReadModel> GetArticle(Guid mediaId, string externalArticleId);
    }
}