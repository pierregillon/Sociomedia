using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;
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
            var keywordValues = command.Articles.Select(x => x.Keywords.Select(a => a.Value)).IntersectAll();

            var keywords = command.Articles
                .SelectMany(x => x.Keywords)
                .Where(x => keywordValues.Contains(x.Value))
                .GroupBy(x => x.Value)
                .Select(g => g.Aggregate((x, y) => x + y))
                .ToArray();

            var theme = new Theme(keywords, command.Articles.Select(x => x.Id).ToArray());

            await _repository.Save(theme);
        }
    }
}