using System;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.ProjectionSynchronizer.Application.Events;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
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
                Url = @event.Url,
                Summary = @event.Summary,
                ImageUrl = @event.ImageUrl,
                PublishDate = @event.PublishDate
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