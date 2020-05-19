using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Articles.Application
{
    public class DeleteArticles : IEventListener<MediaDeleted>
    {
        private readonly IRepository _repository;
        private readonly InMemoryDatabase _database;

        public DeleteArticles(IRepository repository, InMemoryDatabase database)
        {
            _repository = repository;
            _database = database;
        }

        public async Task On(MediaDeleted @event)
        {
            var articleReadModels = _database
                .List<ArticleReadModel>()
                .Where(x => x.MediaId == @event.Id)
                .ToList();

            foreach (var articleReadModel in articleReadModels) {
                var article = await _repository.Get<Article>(articleReadModel.ArticleId);
                article.Delete();
                await _repository.Save(article);
            }
        }
    }
}