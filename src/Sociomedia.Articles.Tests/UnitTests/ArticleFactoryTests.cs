using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain;
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

            var externalArticle = SomeExternalArticle();
            
            var article = await _articleFactory.Build(mediaId, externalArticle);

            article
                .GetUncommittedChanges()
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleImported(
                        default,
                        externalArticle.Title,
                        externalArticle.Summary,
                        externalArticle.PublishDate,
                        externalArticle.Url,
                        externalArticle.ImageUrl,
                        externalArticle.Id,
                        new[]{"some"},
                        mediaId
                    ),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Build_keywords_from_article_content_summary_and_title()
        {
            var mediaId = Guid.NewGuid();

            var externalArticle = new ExternalArticle(
                "",
                "",
                "some title",
                DateTimeOffset.Now,
                "some summary",
                ""
            );

            _webPageDownloader.Download(Arg.Any<string>()).Returns("<html>some content with summary</html>");

            var article = await _articleFactory.Build(mediaId, externalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleImported>()
                .Single()
                .Keywords
                .Should()
                .BeEquivalentTo("some", "summary");
        }

        [Fact]
        public async Task Extract_image_from_article_html_if_no_set_from_feed()
        {
            var externalArticle = SomeExternalArticle();

            externalArticle.ImageUrl = null;

            _webPageDownloader
                .Download(externalArticle.Url)
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
            var externalArticle = SomeExternalArticle("http://www.leparisien.fr/politique/deconfinement-edouard-philippe-donnera-les-details-jeudi-sur-l-etape-du-11-mai-06-05-2020-8312091.php#xtor=RSS-1481423633");

            externalArticle.ImageUrl = null;

            _webPageDownloader
                .Download(externalArticle.Url)
                .Returns(File.ReadAllText("./UnitTests/Resources/leparisien_article_example.html"));

            var article = await _articleFactory.Build(Guid.NewGuid(), externalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleImported>()
                .Single()
                .ImageUrl
                .Should()
                .Be("http://www.leparisien.fr/resizer/V8S9uKi5m2kfPd6k7xcDu_4hdag=/932x582/arc-anglerfish-eu-central-1-prod-leparisien.s3.amazonaws.com/public/RQQRFMFZZRTZJF6D2ZSMVSQTVU.jpg");
        }

        private static ExternalArticle SomeExternalArticle(string url = default)
        {
            return new ExternalArticle(
                "someExternalId",
                url ?? "https://someurl",
                "some title",
                DateTimeOffset.Now,
                "some summary",
                "https://someImageUrl"
            );
        }
    }
}