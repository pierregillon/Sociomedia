using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.AddArticleToTheme
{
    public class AddArticleToThemeCommandHandler : ICommandHandler<AddArticleToThemeCommand>
    {
        private readonly IRepository _repository;

        public AddArticleToThemeCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(AddArticleToThemeCommand command)
        {
            var theme = await _repository.Get<Theme>(command.ThemeId);

            theme.AddArticle(command.Article);

            await _repository.Save(theme);
        }
    }
}