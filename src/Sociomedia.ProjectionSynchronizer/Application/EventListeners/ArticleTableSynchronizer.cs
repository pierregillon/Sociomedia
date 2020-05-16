using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Sociomedia.Articles.Domain;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class ArticleTableSynchronizer : IEventListener<ArticleImported>, IEventListener<ArticleUpdated>
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
                .Value(x => x.MediaId, @event.MediaId)
                .InsertAsync();

            if (@event.Keywords.Count > 10) {
                _dbConnection.BulkCopy(@event.Keywords.Select(x => new KeywordTable {
                    FK_Article = @event.Id,
                    Value = x.Substring(0, Math.Min(x.Length, 50))
                }));
            }
            else {
                foreach (var keyword in @event.Keywords) {
                    await _dbConnection.Keywords
                        .Value(x => x.FK_Article, @event.Id)
                        .Value(x => x.Value, keyword.Substring(0, Math.Min(keyword.Length, 50)))
                        .InsertAsync();
                }
            }
        }

        public async Task On(ArticleUpdated @event)
        {
            await _dbConnection.Articles
                .Where(x => x.Id == @event.Id)
                .Set(x => x.Title, @event.Title)
                .Set(x => x.Url, @event.Url)
                .Set(x => x.Summary, @event.Summary)
                .Set(x => x.ImageUrl, @event.ImageUrl)
                .Set(x => x.PublishDate, @event.PublishDate)
                .UpdateAsync();
        }
    }
}