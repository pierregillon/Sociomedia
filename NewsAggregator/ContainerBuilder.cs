using CQRSlite.Domain;
using CQRSlite.Events;
using NewsAggregator.Application;
using NewsAggregator.Application.Commands.SynchronizeRssFeed;
using NewsAggregator.Application.Queries;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using StructureMap;

namespace NewsAggregator
{
    public class ContainerBuilder
    {
        public static Container Build()
        {
            var container = new Container(x => {
                x.For<IHtmlParser>().Use<HtmlParser>();
                
                x.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();

                x.For<ICommandHandler<SynchronizeRssFeedCommand>>().Use<SynchronizeRssFeedCommandHandler>();
                x.For<IEventListener<ArticleCreated>>().Use(x => x.GetInstance<ReadModelDatabase>());
                x.For<IEventListener<RssSourceAdded>>().Use(x => x.GetInstance<ReadModelDatabase>());
                x.For<IEventListener<RssSourceSynchronized>>().Use(x => x.GetInstance<ReadModelDatabase>());

                x.For<IArticleFinder>().Use<ArticleFinder>();
                x.For<IRssSourceFinder>().Use<RssSourceFinder>();

                x.For(typeof(IRepository)).Use(typeof(Repository));

                x.For<ReadModelDatabase>().Singleton();
                x.For<IEventStore>().Use<InMemoryEventStore>().Singleton();
            });
            return container;
        }
    }
}