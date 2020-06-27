using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ChallengeThemes : IEventListener<ArticleKeywordsDefined>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ThemeChallenger _themeChallenger;
        private readonly ThemeDataFinder _themeDataFinder;

        public ChallengeThemes(ICommandDispatcher commandDispatcher, ThemeChallenger themeChallenger, ThemeDataFinder themeDataFinder)
        {
            _commandDispatcher = commandDispatcher;
            _themeChallenger = themeChallenger;
            _themeDataFinder = themeDataFinder;
        }

        public async Task On(ArticleKeywordsDefined @event)
        {
            var articleToChallenge = GetArticleToChallenge(@event);

            if (articleToChallenge == null) {
                return;
            }

            var commands = _themeChallenger.Challenge(articleToChallenge);

            foreach (var command in commands) {
                await _commandDispatcher.DispatchGeneric(command);
            }
        }

        private ArticleToChallenge GetArticleToChallenge(ArticleKeywordsDefined @event)
        {
            var articleReadModel = _themeDataFinder.GetArticle(@event.Id);
            if (articleReadModel == null) {
                return null;
            }
            return new ArticleToChallenge(@event.Id, articleReadModel.PublishDate, ExtractKeywordsToProcess(@event));
        }

        private static Keyword[] ExtractKeywordsToProcess(ArticleKeywordsDefined @event)
        {
            return @event.Keywords.Select(x => new Keyword(x.Value, x.Occurence)).ToArray();
        }
    }
}