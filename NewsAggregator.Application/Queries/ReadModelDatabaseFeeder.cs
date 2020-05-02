using System.Linq;
using System.Threading.Tasks;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Application.Queries
{
    public class ReadModelDatabaseFeeder : IEventListener<RssSourceAdded>, IEventListener<RssSourceSynchronized>
    {
        private readonly InMemoryDatabase _database;

        public ReadModelDatabaseFeeder(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task On(RssSourceAdded @event)
        {
            _database.Add(new RssSourceReadModel {
                Id = @event.Id,
                Url = @event.Url,
                LastSynchronizationDate = null
            });

            return Task.CompletedTask;
        }

        public Task On(RssSourceSynchronized @event)
        {
            var source = _database.List<RssSourceReadModel>().SingleOrDefault(x => x.Id == @event.Id);
            if (source != null) {
                source.LastSynchronizationDate = @event.SynchronizedDate;
            }
            return Task.CompletedTask;
        }
    }
}