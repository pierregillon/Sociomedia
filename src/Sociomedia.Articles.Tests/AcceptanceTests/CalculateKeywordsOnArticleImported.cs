using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Articles.Tests.UnitTests;
using Sociomedia.Core.Domain;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class CalculateKeywordsOnArticleImported : AcceptanceTests
    {
        [Fact]
        public async Task Generate_keywords_from_article_content_summary_and_title_on_article_imported()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            WebPageDownloader.Download(Arg.Any<string>()).Returns("<html>some content with summary</html>");

            await StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 }
            });

            // Act
            await StoreAndPublish(new IEvent[] {
                new ArticleImported(Guid.NewGuid(), "some title", "some summary", new DateTime(2020, 04, 01), "https://test/article", "", "articleExternalId", new string[0], mediaId) { Version = 1 }
            });

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleKeywordsDefined(
                        default,
                        new[] {
                            new Keyword("some", 3),
                            new Keyword("summary", 2),
                        }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}