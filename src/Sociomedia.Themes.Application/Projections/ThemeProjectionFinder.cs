using System.Threading.Tasks;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeProjectionFinder 
        //: IEventListener<ArticleKeywordsDefined>
        //,
        //IEventListener<ThemeAdded>,
        //IEventListener<ArticleAddedToTheme>
    {
        public readonly ThemeProjection _projection;

        public ThemeProjectionFinder(ThemeProjection projection)
        {
            _projection = projection;
        }

        public Task<ThemeProjection> GetProjection()
        {
            return Task.FromResult(_projection);
        }

        public Task On(ArticleKeywordsDefined @event)
        {
            _projection.AddArticle(@event);

            return Task.CompletedTask;
        }

        public Task On(ThemeAdded @event)
        {
            _projection.AddTheme(@event);

            return Task.CompletedTask;
        }

        public Task On(ArticleAddedToTheme @event)
        {
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}