using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Commands.SynchronizeAllMediaFeeds;
using Sociomedia.Articles.Application.Projections;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Commands.SynchronizeMediaFeeds
{
    public class SynchronizeMediaFeedsCommandHandler : ICommandHandler<SynchronizeAllMediaFeedsCommand>, ICommandHandler<SynchronizeMediaFeedCommand>
    {
        private readonly IRepository _repository;
        private readonly IFeedReader _feedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly ISynchronizationFinder _synchronizationFinder;
        private readonly ILogger _logger;

        public SynchronizeMediaFeedsCommandHandler(
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

        public async Task Handle(SynchronizeMediaFeedCommand command)
        {
            await SynchronizeFeed(new MediaFeedReadModel {
                MediaId = command.MediaId,
                FeedUrl = command.FeedUrl
            });
        }

        private async Task SynchronizeFeed(MediaFeedReadModel feed)
        {
            _logger.Debug("[SYNCHRONIZE_MEDIA_FEED] " + feed.FeedUrl);

            try {
                var externalArticles = await _feedReader.Read(feed.FeedUrl);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(feed.MediaId, externalArticles);
                }
            }
            catch (Exception ex) {
                _logger.Error(ex, "[SYNCHRONIZE_MEDIA_FEED] An error occurs during synchronization of " + feed.FeedUrl);
            }
        }

        private async Task SynchronizeArticles(Guid mediaId, IEnumerable<FeedItem> feedItems)
        {
            foreach (var externalArticle in feedItems) {
                var articleInfo = await _synchronizationFinder.GetArticle(mediaId, externalArticle.Id);
                if (articleInfo == null) {
                    await AddNewArticle(mediaId, externalArticle);
                }
                else if (externalArticle.PublishDate > articleInfo.PublishDate) {
                    await UpdateArticle(externalArticle, articleInfo);
                }
            }
        }

        private async Task AddNewArticle(Guid mediaId, FeedItem feedItem)
        {
            try {
                var article = await _articleFactory.Build(mediaId, feedItem);
                await _repository.Save(article);
            }
            catch (UnreachableWebDocumentException) { }
        }

        private async Task UpdateArticle(FeedItem feedItem, ArticleReadModel articleInfo)
        {
            var article = await _repository.Get<Article>(articleInfo.ArticleId);

            article.Update(feedItem);

            await _repository.Save(article);
        }
    }
}