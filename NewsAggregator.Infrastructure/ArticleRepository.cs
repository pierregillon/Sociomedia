using System.Collections.Generic;
using System.Threading.Tasks;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Infrastructure.CQRS;

namespace NewsAggregator.Infrastructure
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IEventPublisher _eventPublisher;

        public ArticleRepository(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public Task<IReadOnlyCollection<Article>> GetAll(string rssSourceId)
        {
            return Task.FromResult(new List<Article>()).ContinueWith(x => (IReadOnlyCollection<Article>) x.Result);
        }

        public Task Save(Article article)
        {
            return _eventPublisher.Publish(article.UncommitedEvents);
        }
    }
}