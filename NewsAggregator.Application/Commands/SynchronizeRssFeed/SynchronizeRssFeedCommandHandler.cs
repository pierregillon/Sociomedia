using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using NewsAggregator.Application.Queries;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Application.Commands.SynchronizeRssFeed
{
    public class SynchronizeRssFeedCommandHandler : ICommandHandler<SynchronizeRssFeedCommand>
    {
        private readonly IRepository _repository;
        private readonly IRssFeedReader _rssFeedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly IRssSourceFinder _rssSourceFinder;

        public SynchronizeRssFeedCommandHandler(
            IRepository repository,
            IRssFeedReader rssFeedReader,
            ArticleFactory articleFactory,
            IRssSourceFinder rssSourceFinder)
        {
            _repository = repository;
            _rssFeedReader = rssFeedReader;
            _articleFactory = articleFactory;
            _rssSourceFinder = rssSourceFinder;
        }

        public async Task Handle(SynchronizeRssFeedCommand command)
        {
            var rssSources = await _rssSourceFinder.GetAll();
            foreach (var source in rssSources) {
                var feeds = await _rssFeedReader.ReadNewFeeds(source.Url, source.LastSynchronizationDate);
                if (feeds.Any()) {
                    await SynchronizeArticles(source.Id, feeds);
                }
            }
        }

        private async Task SynchronizeArticles(Guid sourceId, RssFeeds feeds)
        {
            foreach (var rssFeed in feeds) {
                await _repository.Save(_articleFactory.Build(rssFeed.Url, rssFeed.Html, sourceId));
            }

            var source = await _repository.Get<RssSource>(sourceId);
            source.Synchronize();
            await _repository.Save(source);
        }
    }
}