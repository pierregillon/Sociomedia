using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class ArticleFactoryTests
    {
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly ArticleFactory _articleFactory;

        public ArticleFactoryTests()
        {
            _webPageDownloader = Substitute.For<IWebPageDownloader>();
            _webPageDownloader.Download(Arg.Any<string>()).Returns("<html></html>");

            var keywordDictionary = Substitute.For<IKeywordDictionary>();
            keywordDictionary.IsValidKeyword(Arg.Any<string>()).Returns(true);

            _articleFactory = new ArticleFactory(new HtmlParser(), _webPageDownloader, new KeywordsParser(keywordDictionary));
        }

        [Fact]
        public async Task Build_new_article_from_external_article()
        {
            var mediaId = Guid.NewGuid();

            var feedItem = SomeFeedItem();

            var article = await _articleFactory.Build(mediaId, feedItem);

            article
                .GetUncommittedChanges()
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleImported(
                        default,
                        feedItem.Title,
                        feedItem.Summary,
                        feedItem.PublishDate,
                        feedItem.Link,
                        feedItem.ImageUrl,
                        feedItem.Id,
                        Array.Empty<string>(),
                        mediaId
                    ),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Extract_image_from_article_html_if_no_set_from_feed()
        {
            var externalArticle = SomeFeedItem();

            externalArticle.ImageUrl = null;

            _webPageDownloader
                .Download(externalArticle.Link)
                .Returns(File.ReadAllText("./UnitTests/Resources/marianne_article_example.html"));

            var article = await _articleFactory.Build(Guid.NewGuid(), externalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleImported>()
                .Single()
                .ImageUrl
                .Should()
                .Be("https://media.marianne.net/sites/default/files/styles/mrn_article_large/public/sport-amateur-marianne-menace.jpg");
        }

        [Fact]
        public async Task Make_extracted_image_url_absolute()
        {
            var feedItem = SomeFeedItem("http://www.leparisien.fr/politique/deconfinement-edouard-philippe-donnera-les-details-jeudi-sur-l-etape-du-11-mai-06-05-2020-8312091.php#xtor=RSS-1481423633");

            feedItem.ImageUrl = null;

            _webPageDownloader
                .Download(feedItem.Link)
                .Returns(File.ReadAllText("./UnitTests/Resources/leparisien_article_example.html"));

            var article = await _articleFactory.Build(Guid.NewGuid(), feedItem);

            article
                .GetUncommittedChanges()
                .OfType<ArticleImported>()
                .Single()
                .ImageUrl
                .Should()
                .Be("http://www.leparisien.fr/resizer/V8S9uKi5m2kfPd6k7xcDu_4hdag=/932x582/arc-anglerfish-eu-central-1-prod-leparisien.s3.amazonaws.com/public/RQQRFMFZZRTZJF6D2ZSMVSQTVU.jpg");
        }

        private static FeedItem SomeFeedItem(string url = default)
        {
            return new FeedItem {
                Id = "someExternalId",
                Link = url ?? "https://someurl",
                Title = "some title",
                PublishDate = DateTimeOffset.Now,
                Summary = "some summary",
                ImageUrl = "https://someImageUrl"
            };
        }
    }
}