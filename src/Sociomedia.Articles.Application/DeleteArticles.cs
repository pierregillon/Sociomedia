using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Application.Commands.DeleteArticle;
using Sociomedia.Articles.Application.Projections;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Articles.Application
{
    public class DeleteArticles : IEventListener<MediaDeleted>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly InMemoryDatabase _database;

        public DeleteArticles(ICommandDispatcher commandDispatcher, InMemoryDatabase database)
        {
            _commandDispatcher = commandDispatcher;
            _database = database;
        }

        public async Task On(MediaDeleted @event)
        {
            var articleReadModels = _database
                .List<ArticleReadModel>()
                .Where(x => x.MediaId == @event.Id)
                .ToList();

            foreach (var articleReadModel in articleReadModels) {
                await _commandDispatcher.Dispatch(new DeleteArticleCommand(articleReadModel.ArticleId));
            }
        }
    }
}