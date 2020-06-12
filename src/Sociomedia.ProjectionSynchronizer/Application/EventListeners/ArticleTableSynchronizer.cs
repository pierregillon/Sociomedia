using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class ArticleTableSynchronizer :
        IEventListener<ArticleImported>,
        IEventListener<ArticleUpdated>,
        IEventListener<ArticleKeywordsDefined>,
        IEventListener<ArticleDeleted>
    {
        private readonly DbConnectionReadModel _dbConnection;

        public ArticleTableSynchronizer(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task On(ArticleImported @event)
        {
            if (string.IsNullOrWhiteSpace(@event.Title)) {
                return;
            }

            await _dbConnection.Articles
                .Value(x => x.Id, @event.Id)
                .Value(x => x.Title, @event.Title)
                .Value(x => x.Url, @event.Url)
                .Value(x => x.Summary, @event.Summary)
                .Value(x => x.ImageUrl, @event.ImageUrl)
                .Value(x => x.PublishDate, @event.PublishDate)
                .Value(x => x.MediaId, @event.MediaId)
                .Value(x => x.Keywords, ToKeywordBlock(@event.Keywords))
                .InsertAsync();
        }

        public async Task On(ArticleUpdated @event)
        {
            if (string.IsNullOrWhiteSpace(@event.Title)) {
                return;
            }

            await _dbConnection.Articles
                .Where(x => x.Id == @event.Id)
                .Set(x => x.Title, @event.Title)
                .Set(x => x.Url, @event.Url)
                .Set(x => x.Summary, @event.Summary)
                .Set(x => x.ImageUrl, @event.ImageUrl)
                .Set(x => x.PublishDate, @event.PublishDate)
                .UpdateAsync();
        }

        public async Task On(ArticleKeywordsDefined @event)
        {
            await _dbConnection.Articles
                .Where(x => x.Id == @event.Id)
                .Set(x => x.Keywords, ToKeywordBlock(@event.Keywords.Select(x => x.Value)))
                .UpdateAsync();
        }

        public async Task On(ArticleDeleted @event)
        {
            await _dbConnection.Articles
                .Where(x => x.Id == @event.Id)
                .DeleteAsync();
        }

        private static string ToKeywordBlock(IEnumerable<string> keywords)
        {
            return keywords.ConcatWords().ToLower().RemoveDiacritics();
        }
    }
}