using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Projections
{
    public class ArticleProjection :
        Projection<ArticleReadModel>,
        IEventListener<ArticleImported>,
        IEventListener<ArticleUpdated>,
        IEventListener<ArticleDeleted>
    {
        public ArticleProjection(InMemoryDatabase database, ILogger logger) : base(database, logger) { }

        public async Task On(ArticleImported @event)
        {
            await Add(new ArticleReadModel {
                MediaId = @event.MediaId,
                ExternalArticleId = @event.ExternalArticleId,
                ArticleId = @event.Id,
                PublishDate = @event.PublishDate
            });
        }

        public async Task On(ArticleUpdated @event)
        {
            var source = (await GetAll())
                .SingleOrDefault(x => x.ArticleId == @event.Id);

            if (source != null) {
                source.PublishDate = @event.PublishDate;
            }
            else {
                LogError($"Unable to find article '{@event.Id}' to update.");
            }
        }

        public async Task On(ArticleDeleted @event)
        {
            var articles = (await GetAll()).Where(x => x.ArticleId == @event.Id);

            foreach (var article in articles) {
                await Remove(article);
            }
        }
    }
}