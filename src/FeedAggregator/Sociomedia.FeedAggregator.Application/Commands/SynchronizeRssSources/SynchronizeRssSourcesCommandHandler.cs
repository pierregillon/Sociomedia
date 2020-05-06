using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Application.Commands.SynchronizeRssSources
{
    public class SynchronizeRssSourcesCommandHandler : ICommandHandler<SynchronizeRssSourcesCommand>
    {
        private readonly IRepository _repository;
        private readonly IRssSourceReader _rssSourceReader;
        private readonly ArticleFactory _articleFactory;
        private readonly IRssSourceFinder _rssSourceFinder;

        public SynchronizeRssSourcesCommandHandler(
            IRepository repository,
            IRssSourceReader rssSourceReader,
            ArticleFactory articleFactory,
            IRssSourceFinder rssSourceFinder)
        {
            _repository = repository;
            _rssSourceReader = rssSourceReader;
            _articleFactory = articleFactory;
            _rssSourceFinder = rssSourceFinder;
        }

        public async Task Handle(SynchronizeRssSourcesCommand command)
        {
            var rssSources = await _rssSourceFinder.GetAll();
            foreach (var source in rssSources) {
                var externalArticles = await _rssSourceReader.ReadNewArticles(source.Url, source.LastSynchronizationDate);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(source.Id, externalArticles);
                    await UpdateLastSynchronizationDate(source.Id);
                }
            }
        }

        private async Task SynchronizeArticles(Guid sourceId, IEnumerable<ExternalArticle> externalArticles)
        {
            foreach (var externalArticle in externalArticles) {
                var article = await _articleFactory.Build(sourceId, externalArticle);
                await _repository.Save(article);
            }
        }

        private async Task UpdateLastSynchronizationDate(Guid sourceId)
        {
            var source = await _repository.Get<RssSource>(sourceId);
            
            source.Synchronize();
            
            await _repository.Save(source);
        }
    }
}