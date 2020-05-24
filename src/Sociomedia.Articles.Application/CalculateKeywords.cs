using System.Threading.Tasks;
using Sociomedia.Articles.Application.Commands.CalculateKeywords;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;

namespace Sociomedia.Articles.Application
{
    public class CalculateKeywords : IEventListener<ArticleImported>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public CalculateKeywords(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(ArticleImported @event)
        {
            await _commandDispatcher.Dispatch(new CalculateKeywordsCommand(@event.Id));
        }
    }
}