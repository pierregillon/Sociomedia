using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Application.Queries
{
    public class ReadModelDatabase : IEventListener<ArticleCreated>, IEventListener<RssSourceAdded>, IEventListener<RssSourceSynchronized>
    {
        public readonly List<ArticleReadModel> Articles = new List<ArticleReadModel>();
        public readonly List<RssSourceReadModel> Sources = new List<RssSourceReadModel>();

        public Task On(ArticleCreated @event)
        {
            Articles.Add(new ArticleReadModel {
                Url = @event.Url,
                RssSourceId = @event.RssSourceId
            });

            return Task.CompletedTask;
        }

        public Task On(RssSourceAdded @event)
        {
            Sources.Add(new RssSourceReadModel {
                Id = @event.Id,
                Url = @event.Url,
                LastSynchronizationDate = null
            });

            return Task.CompletedTask;
        }

        public Task On(RssSourceSynchronized @event)
        {
            var source = Sources.SingleOrDefault(x => x.Id == @event.Id);
            if (source != null) {
                source.LastSynchronizationDate = @event.SynchronizedDate;
            }
            return Task.CompletedTask;
        }
    }
}