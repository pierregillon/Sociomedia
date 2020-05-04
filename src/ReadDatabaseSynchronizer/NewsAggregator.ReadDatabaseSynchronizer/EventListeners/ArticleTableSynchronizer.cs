using System;
using System.Threading.Tasks;
using LinqToDB;
using NewsAggregator.ReadDatabaseSynchronizer.Events;
using NewsAggregator.ReadDatabaseSynchronizer.ReadModels;

namespace NewsAggregator.ReadDatabaseSynchronizer.EventListeners
{
    public class ArticleTableSynchronizer : IEventListener<ArticleSynchronized>
    {
        private readonly DbConnectionReadModel _connection;

        public ArticleTableSynchronizer(DbConnectionReadModel connection)
        {
            _connection = connection;
        }

        public async Task On(ArticleSynchronized @event)
        {
            await _connection.Articles.InsertAsync(() => new ArticleTable {
                Id = @event.Id,
                Title = @event.Title,
                Url = @event.Url.AbsoluteUri
            });

            foreach (var keyword in @event.Keywords) {
                await _connection.Keywords.InsertAsync(() => new KeywordTable {
                    FK_Article = @event.Id,
                    Value = keyword.Substring(0, Math.Min(keyword.Length, 50))
                });
            }
        }
    }
}