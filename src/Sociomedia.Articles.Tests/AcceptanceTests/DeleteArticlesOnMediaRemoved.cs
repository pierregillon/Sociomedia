using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Tests.UnitTests;
using Sociomedia.Core.Domain;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class DeleteArticlesOnMediaRemoved : AcceptanceTests
    {
        [Fact]
        public async Task Delete_articles_on_media_removed()
        {
            // Arrange
            var mediaId = Guid.NewGuid();
            var articleId1 = Guid.NewGuid();
            var articleId2 = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(articleId1, "some title", "some summary", DateTimeOffset.Now, "http://test.com", "http://test.jpg", "somexternalarticle", new string[0], mediaId) { Version = 1 },
                new ArticleImported(articleId2, "some title", "some summary", DateTimeOffset.Now, "http://test.com", "http://test.jpg", "somexternalarticle", new string[0], mediaId) { Version = 1 },
            });

            EventStore.CommitEvents();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaDeleted(mediaId)
            });

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleDeleted(articleId1),
                    new ArticleDeleted(articleId2)
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}