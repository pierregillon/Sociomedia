using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Application;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.Infrastructure;

namespace Sociomedia.FeedAggregator.Application.SynchronizeAllMediaFeeds
{
    public class SynchronizeAllMediaFeedsCommandHandler : ICommandHandler<SynchronizeAllMediaFeedsCommand>
    {
        private readonly IRepository _repository;
        private readonly IFeedReader _feedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly IMediaFeedFinder _mediaFeedFinder;

        public SynchronizeAllMediaFeedsCommandHandler(
            IRepository repository,
            IFeedReader feedReader,
            ArticleFactory articleFactory,
            IMediaFeedFinder mediaFeedFinder)
        {
            _repository = repository;
            _feedReader = feedReader;
            _articleFactory = articleFactory;
            _mediaFeedFinder = mediaFeedFinder;
        }

        public async Task Handle(SynchronizeAllMediaFeedsCommand command)
        {
            var mediaFeeds = await _mediaFeedFinder.GetAll();
            foreach (var feed in mediaFeeds) {
                var externalArticles = await _feedReader.ReadArticles(feed.FeedUrl);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(feed.MediaId, externalArticles);
                    await UpdateLastSynchronizationDate(feed);
                }
            }
        }

        private async Task SynchronizeArticles(Guid mediaId, IEnumerable<ExternalArticle> externalArticles)
        {
            foreach (var externalArticle in externalArticles) {
                var articleInfo = await _mediaFeedFinder.GetArticle(mediaId, externalArticle.Id);
                if (articleInfo == null) {
                    await AddNewArticle(mediaId, externalArticle);
                }
                else {
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

        private async Task UpdateLastSynchronizationDate(MediaFeedReadModel mediaFeed)
        {
            var media = await _repository.Get<Media>(mediaFeed.MediaId);

            media.SynchronizeFeed(mediaFeed.FeedUrl);

            await _repository.Save(media);
        }
    }
}