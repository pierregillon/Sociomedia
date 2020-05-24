using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Application.Projections;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class InitializeProjections : AcceptanceTests
    {
        private readonly ProjectionsBootstrapper _projectionsBootstrapper;
        private readonly IEventStoreExtended _eventStore = Substitute.For<IEventStoreExtended>();
        private readonly ISynchronizationFinder _finder;

        public InitializeProjections()
        {
            Container.Inject(_eventStore);

            _projectionsBootstrapper = Container.GetInstance<ProjectionsBootstrapper>();
            _finder = Container.GetInstance<ISynchronizationFinder>();
        }

        [Fact]
        public async Task Media_feed_projection_is_initialized_from_bootstrap()
        {
            var mediaId = Guid.NewGuid();

            _eventStore
                .GetAllEventsBetween(Arg.Any<Position>(), Arg.Any<Position>(), Arg.Any<IReadOnlyCollection<Type>>())
                .Returns(new AsyncList<IEvent> {
                    new MediaFeedAdded(mediaId, "https://site/rss.xml"),
                    new MediaFeedAdded(mediaId, "https://site/atom.xml"),
                    new MediaFeedAdded(mediaId, "https://site/rss1.xml"),
                    new MediaFeedRemoved(mediaId, "https://site/rss1.xml"),
                });

            await _projectionsBootstrapper.InitializeUntil(0);

            (await _finder.GetAllMediaFeeds())
                .Should()
                .BeEquivalentTo(new[] {
                    new MediaFeedReadModel {
                        MediaId = mediaId,
                        FeedUrl = "https://site/rss.xml"
                    },
                    new MediaFeedReadModel {
                        MediaId = mediaId,
                        FeedUrl = "https://site/atom.xml"
                    }
                });
        }

        [Fact]
        public async Task Article_projection_is_initialized_from_bootstrap()
        {
            var articleId = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var mediaId = Guid.NewGuid();

            _eventStore
                .GetAllEventsBetween(Arg.Any<Position>(), Arg.Any<Position>(), Arg.Any<IReadOnlyCollection<Type>>())
                .Returns(new AsyncList<IEvent> {
                    new ArticleImported(articleId, "test", "summary", DateTimeOffset.Now.Date.Subtract(TimeSpan.FromDays(1)), "https://site.html", "https://site/image.jpg", "externalId", new string[0], mediaId),
                    new ArticleUpdated(articleId, "some title", "some article", DateTimeOffset.Now.Date, "https://site2.html", "https://site2/image.jpg"),
                    new ArticleImported(article2Id, "test", "summary", DateTimeOffset.Now.Date.Subtract(TimeSpan.FromDays(1)), "https://site.html", "https://site/image.jpg", "externalId", new string[0], mediaId),
                    new ArticleDeleted(article2Id)
                });

            await _projectionsBootstrapper.InitializeUntil(0);

            (await _finder.GetArticle(mediaId, "externalId"))
                .Should()
                .BeEquivalentTo(
                    new ArticleReadModel {
                        ArticleId = articleId,
                        ExternalArticleId = "externalId",
                        MediaId = mediaId,
                        PublishDate = DateTimeOffset.Now.Date
                    });
        }
    }
}