using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public interface IMediaFeedFinder
    {
        Task<IReadOnlyCollection<MediaFeedReadModel>> GetAll();
        Task<ArticleReadModel> GetArticle(Guid mediaId, string externalArticleId);
    }
}