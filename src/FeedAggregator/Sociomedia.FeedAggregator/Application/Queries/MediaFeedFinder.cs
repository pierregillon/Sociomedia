using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public class MediaFeedFinder : IMediaFeedFinder
    {
        private readonly InMemoryDatabase _database;

        public MediaFeedFinder(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task<IReadOnlyCollection<MediaFeedReadModel>> GetAll()
        {
            await Task.Delay(0);

            return _database.List<MediaFeedReadModel>().ToArray();
        }

        public async Task<ArticleReadModel> GetArticle(Guid mediaId, string externalArticleId)
        {
            await Task.Delay(0);

            return _database
                .List<ArticleReadModel>()
                .Where(x => x.MediaId == mediaId)
                .FirstOrDefault(x => x.ExternalArticleId == externalArticleId);
        }
    }
}