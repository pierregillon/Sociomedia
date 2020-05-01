using NewsAggregator.Application;
using NewsAggregator.Application.SynchronizeRssFeed;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using StructureMap;

namespace NewsAggregator {
    public class ContainerBuilder
    {
        public static Container Build()
        {
            var container = new Container(x => {
                x.For<IHtmlParser>().Use<HtmlParser>();
                x.For<IArticleRepository>().Use<ArticleRepository>();
                x.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();
                x.For<ICommandHandler<SynchronizeRssFeedCommand>>().Use<SynchronizeRssFeedCommandHandler>();
            });
            return container;
        }
    }
}