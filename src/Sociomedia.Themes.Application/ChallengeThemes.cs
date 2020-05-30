using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
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
        private readonly IRepository _repository;

        public ChallengeThemes(ICommandDispatcher commandDispatcher, ThemeProjectionFinder themeProjectionFinder, IRepository repository)
        {
            _commandDispatcher = commandDispatcher;
            _themeProjectionFinder = themeProjectionFinder;
            _repository = repository;
        }

        public async Task On(ArticleKeywordsDefined @event)
        {
            var themeProjection = await _themeProjectionFinder.GetProjection();

            themeProjection.AddArticle(@event);

            var themeManager = new ThemeManager2(themeProjection);

            var @events = themeManager.Add(new Article(@event.Id, @event.Keywords.Select(x => new Keyword2(x.Value, x.Occurence)).ToArray())).ToArray();

            //themeProjection.Apply(events);

            foreach (var group in @events.GroupBy(x => x.Id)) {
                var existingTheme = await GetTheme(@group);
                if (existingTheme != null) {
                    existingTheme.ApplyChanges(group);
                }
                else {
                    existingTheme = new Theme(group);
                }
                await _repository.Save(existingTheme);
            }


            //foreach (var command in commands) {
            //    await _commandDispatcher.DispatchGeneric(command);
            //}
        }

        private async Task<Theme> GetTheme(IGrouping<Guid, ThemeEvent> @group)
        {
            try {
                return await _repository.Get<Theme>(@group.Key);
            }
            catch (AggregateNotFoundException ex) {
                return null;
            }
        }
    }
}