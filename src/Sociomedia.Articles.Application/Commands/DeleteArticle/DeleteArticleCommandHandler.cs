using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Commands.DeleteArticle
{
    public class DeleteArticleCommandHandler : ICommandHandler<DeleteArticleCommand>
    {
        private readonly IRepository _repository;

        public DeleteArticleCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteArticleCommand command)
        {
            var article = await _repository.Get<Article>(command.ArticleId);

            article.Delete();

            await _repository.Save(article);
        }
    }
}