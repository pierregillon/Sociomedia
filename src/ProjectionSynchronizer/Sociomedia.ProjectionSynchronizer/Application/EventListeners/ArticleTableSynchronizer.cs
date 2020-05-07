using System;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Domain.Articles;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class ArticleTableSynchronizer : IEventListener<ArticleImported>
    {
        private readonly DbConnectionReadModel _dbConnection;

        public ArticleTableSynchronizer(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task On(ArticleImported @event)
        {
            await _dbConnection.Articles
                .Value(x => x.Id, @event.Id)
                .Value(x => x.Title, @event.Title)
                .Value(x => x.Url, @event.Url)
                .Value(x => x.Summary, @event.Summary)
                .Value(x => x.ImageUrl, @event.ImageUrl)
                .Value(x => x.PublishDate, @event.PublishDate)
                .InsertAsync();

            foreach (var keyword in @event.Keywords) {
                await _dbConnection.Keywords
                    .Value(x => x.FK_Article, @event.Id)
                    .Value(x => x.Value, keyword.Substring(0, Math.Min(keyword.Length, 50)))
                    .InsertAsync();
            }
        }
    }
}