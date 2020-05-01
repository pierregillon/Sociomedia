using System.Linq;
using System.Threading.Tasks;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Application.SynchronizeRssFeed
{
    public class SynchronizeRssFeedCommandHandler : ICommandHandler<SynchronizeRssFeedCommand>
    {
        private readonly IRssSourceRepository _rssSourceRepository;
        private readonly IRssFeedReader _rssFeedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly IArticleRepository _articleRepository;

        public SynchronizeRssFeedCommandHandler(IRssSourceRepository rssSourceRepository, IRssFeedReader rssFeedReader, ArticleFactory articleFactory, IArticleRepository articleRepository)
        {
            _rssSourceRepository = rssSourceRepository;
            _rssFeedReader = rssFeedReader;
            _articleFactory = articleFactory;
            _articleRepository = articleRepository;
        }

        public async Task Handle(SynchronizeRssFeedCommand command)
        {
            var rssSources = await _rssSourceRepository.GetAll();
            foreach (var rssSource in rssSources) {
                var feeds = await _rssFeedReader.Read(rssSource);
                if (feeds.LastUpdateDate > rssSource.LastSynchronizationDate) {
                    await CreateNewArticles(rssSource.Id, feeds);
                    rssSource.Synchronized();
                    await _rssSourceRepository.Save(rssSource);
                }
            }
        }

        private async Task CreateNewArticles(string rssSourceId, RssFeeds feeds)
        {
            var articles = await _articleRepository.GetAll(rssSourceId);
            foreach (var rssFeed in feeds) {
                if (articles.Any(x => x.RssFeedId == rssFeed.Id)) continue;
                var article = _articleFactory.Build(rssFeed.Url, rssFeed.Html);
                await _articleRepository.Save(article);
            }
        }
    }
}