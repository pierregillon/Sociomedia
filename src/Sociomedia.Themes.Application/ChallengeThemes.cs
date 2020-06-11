using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Themes.Domain;
using Article = Sociomedia.Themes.Domain.Article;

namespace Sociomedia.Themes.Application
{
    public class ChallengeThemes : IEventListener<ArticleKeywordsDefined>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ThemeManager _themeManager;

        public ChallengeThemes(ICommandDispatcher commandDispatcher, ThemeManager themeManager)
        {
            _commandDispatcher = commandDispatcher;
            _themeManager = themeManager;
        }

        public async Task On(ArticleKeywordsDefined @event)
        {
            var article = new Article(@event.Id, ExtractKeywordsToProcess(@event));

            var commands = _themeManager.Add(article);

            foreach (var command in commands) {
                await _commandDispatcher.DispatchGeneric(command);
            }
        }

        private static Keyword[] ExtractKeywordsToProcess(ArticleKeywordsDefined @event)
        {
            return @event.Keywords.Select(x => new Keyword(x.Value, x.Occurence)).ToArray();
        }
    }
}