using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;
using Article = Sociomedia.Themes.Domain.Article;

namespace Sociomedia.Themes.Application
{
    public class ChallengeThemes : IEventListener<ArticleKeywordsDefined>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ThemeProjectionFinder _themeProjectionFinder;

        public ChallengeThemes(ICommandDispatcher commandDispatcher, ThemeProjectionFinder themeProjectionFinder)
        {
            _commandDispatcher = commandDispatcher;
            _themeProjectionFinder = themeProjectionFinder;
        }

        public async Task On(ArticleKeywordsDefined @event)
        {
            var themeProjection = await _themeProjectionFinder.GetProjection();

            var themeManager = new ThemeManager2(themeProjection);

            var commands = themeManager.Add(new Article(@event.Id, @event.Keywords.Select(x=>new Keyword2(x.Value, x.Occurence)).ToArray()));

            foreach (var command in commands) {
                await _commandDispatcher.DispatchGeneric(command);
            }
        }
    }
}