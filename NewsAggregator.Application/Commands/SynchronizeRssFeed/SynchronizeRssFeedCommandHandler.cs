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
        private readonly IArticleFinder _articleFinder;
        private readonly IRssSourceFinder _rssSourceFinder;

        public SynchronizeRssFeedCommandHandler(
            IRepository repository,
            IRssFeedReader rssFeedReader,
            ArticleFactory articleFactory,
            IArticleFinder articleFinder,
            IRssSourceFinder rssSourceFinder)
        {
            _repository = repository;
            _rssFeedReader = rssFeedReader;
            _articleFactory = articleFactory;
            _articleFinder = articleFinder;
            _rssSourceFinder = rssSourceFinder;
        }

        public async Task Handle(SynchronizeRssFeedCommand command)
        {
            var rssSources = await _rssSourceFinder.GetAll();
            foreach (var source in rssSources) {
                var feeds = await _rssFeedReader.Read(source.Url);
                if (feeds.LastPublishDate > source.LastSynchronizationDate) {
                    var test = await _repository.Get<RssSource>(source.Id);
                    await CreateNewArticles(source.Id, feeds);
                    test.Synchronize();
                    await _repository.Save(test);
                }
            }
        }

        private async Task CreateNewArticles(Guid rssSourceId, RssFeeds feeds)
        {
            var articles = await _articleFinder.GetArticles(rssSourceId);
            foreach (var rssFeed in feeds) {
                if (articles.Any(x => x.Url == rssFeed.Id)) {
                    continue;
                }
                var article = _articleFactory.Build(rssFeed.Url, rssFeed.Html, rssSourceId);
                await _repository.Save(article);
            }
        }
    }
}