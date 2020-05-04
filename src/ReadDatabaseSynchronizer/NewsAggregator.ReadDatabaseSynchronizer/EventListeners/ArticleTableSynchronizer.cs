using System;
using System.Threading.Tasks;
using NewsAggregator.ReadDatabaseSynchronizer.Events;

namespace NewsAggregator.ReadDatabaseSynchronizer.EventListeners
{
    public class ArticleTableSynchronizer : IEventListener<ArticleSynchronized>
    {
        public Task On(ArticleSynchronized @event)
        {
            throw new NotImplementedException();
        }
    }
}