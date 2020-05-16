using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using EventStore.ClientAPI;
using Sociomedia.Application.Application;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;

namespace Sociomedia.Articles.Application.Commands.SynchronizeAllMediaFeeds
{
    public class SynchronizeAllMediaFeedsCommandHandler : ICommandHandler<SynchronizeAllMediaFeedsCommand>
    {
        private readonly IRepository _repository;
        private readonly IFeedReader _feedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly ISynchronizationFinder _synchronizationFinder;
        private readonly ILogger _logger;

        public SynchronizeAllMediaFeedsCommandHandler(
            IRepository repository,
            IFeedReader feedReader,
            ArticleFactory articleFactory,
            ISynchronizationFinder synchronizationFinder,
            ILogger logger)
        {
            _repository = repository;
            _feedReader = feedReader;
            _articleFactory = articleFactory;
            _synchronizationFinder = synchronizationFinder;
            _logger = logger;
        }

        public async Task Handle(SynchronizeAllMediaFeedsCommand command)
        {
            var mediaFeeds = await _synchronizationFinder.GetAllMediaFeeds();
            foreach (var feed in mediaFeeds) {
                await SynchronizeFeed(feed);
            }
        }

        private async Task SynchronizeFeed(MediaFeedReadModel feed)
        {
            _logger.Debug("[SYNCHRONIZE_MEDIA_FEED] " + feed.FeedUrl);

            try {
                var externalArticles = await _feedReader.ReadArticles(feed.FeedUrl);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(feed.MediaId, externalArticles);
                }
            }
            catch (Exception ex) {
                _logger.Error(ex, "[SYNCHRONIZE_MEDIA_FEED] An error occurs during synchronization of " + feed.FeedUrl);
            }
        }

        private async Task SynchronizeArticles(Guid mediaId, IEnumerable<ExternalArticle> externalArticles)
        {
            foreach (var externalArticle in externalArticles) {
                var articleInfo = await _synchronizationFinder.GetArticle(mediaId, externalArticle.Id);
                if (articleInfo == null) {
                    await AddNewArticle(mediaId, externalArticle);
                }
                else if(externalArticle.PublishDate > articleInfo.PublishDate) {
                    await UpdateArticle(externalArticle, articleInfo);
                }
            }
        }

        private async Task AddNewArticle(Guid mediaId, ExternalArticle externalArticle)
        {
            try {
                var article = await _articleFactory.Build(mediaId, externalArticle);
                await _repository.Save(article);
            }
            catch (UnreachableWebDocumentException) { }
        }

        private async Task UpdateArticle(ExternalArticle externalArticle, ArticleReadModel articleInfo)
        {
            var article = await _repository.Get<Article>(articleInfo.ArticleId);
            
            article.Update(externalArticle);
            
            await _repository.Save(article);
        }
    }
}