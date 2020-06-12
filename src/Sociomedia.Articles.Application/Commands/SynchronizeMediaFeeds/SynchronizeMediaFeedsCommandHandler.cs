using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Projections;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Core;
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
            try {
                Debug("Start all feeds synchronization");
                var mediaFeeds = await _synchronizationFinder.GetAllMediaFeeds();
                foreach (var feed in mediaFeeds) {
                    await SynchronizeFeed(feed);
                }
            }
            finally {
                Debug("All feeds synchronization ended");
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
            try {
                Debug($"Synchronizing {feed.FeedUrl}");
                var externalArticles = await _feedReader.Read(feed.FeedUrl);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(feed.MediaId, externalArticles);
                }
            }
            catch (Exception ex) {
                Error(ex, "An error occurs during synchronization of " + feed.FeedUrl);
            }
        }

        private async Task SynchronizeArticles(Guid mediaId, IEnumerable<FeedItem> feedItems)
        {
            foreach (var feedItem in feedItems.Where(IsConsistentForSynchronization)) {
                var articleInfo = await _synchronizationFinder.GetArticle(mediaId, feedItem.Id);
                if (articleInfo == null) {
                    await AddNewArticle(mediaId, feedItem);
                }
                else if (feedItem.PublishDate > articleInfo.PublishDate) {
                    await UpdateArticle(feedItem, articleInfo);
                }
            }
        }

        private bool IsConsistentForSynchronization(FeedItem feedItem)
        {
            if (string.IsNullOrWhiteSpace(feedItem.Link)) {
                Error($"Unable to import feed {feedItem.Id} : link was not defined.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(feedItem.Id)) {
                Error($"Unable to import feed {feedItem.Link} : id was not defined.");
                return false;
            }
            if (!feedItem.PublishDate.HasValue) {
                Error($"Unable to import feed {feedItem.Link} : publish date was not defined.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(feedItem.Title)) {
                Error($"Unable to import feed {feedItem.Link} : title was not defined.");
                return false;
            }
            return true;
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

            article.UpdateFromFeed(feedItem);

            await _repository.Save(article);
        }

        private void Debug(string message)
        {
            _logger.Debug($"[{GetType().DisplayableName()}] {message}");
        }

        private void Error(Exception ex, string error)
        {
            _logger.Error(ex, $"[{GetType().DisplayableName()}] {error}");
        }

        private void Error(string error)
        {
            _logger.Error($"[{GetType().DisplayableName()}] {error}");
        }
    }
}