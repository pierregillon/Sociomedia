using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.CreateNewTheme
{
    public class CreateNewThemeCommandHandler : ICommandHandler<CreateNewThemeCommand>
    {
        private readonly IRepository _repository;

        public CreateNewThemeCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateNewThemeCommand command)
        {
            var theme = new Theme(command.Keywords, command.Articles);

            await _repository.Save(theme);
        }
    }
}