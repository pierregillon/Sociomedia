using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.Infrastructure;
using Sociomedia.Tests.Features;
using Xunit;

namespace Sociomedia.Tests
{
    public class ArticleFactoryTests
    {
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly ArticleFactory _articleFactory;

        public ArticleFactoryTests()
        {
            _webPageDownloader = Substitute.For<IWebPageDownloader>();
            _webPageDownloader.Download(Arg.Any<string>()).Returns("<html></html>");

            _articleFactory = new ArticleFactory(new HtmlParser(), _webPageDownloader);
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
                        new string[0],
                        mediaId
                    ),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Extract_image_from_article_html_if_no_set_from_feed()
        {
            var externalArticle = SomeExternalArticle();

            externalArticle.ImageUrl = null;

            _webPageDownloader
                .Download(externalArticle.Url)
                .Returns(File.ReadAllText("./Resources/marianne_article_example.html"));

            var article = await _articleFactory.Build(Guid.NewGuid(), externalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleImported>()
                .Single()
                .ImageUrl
                .Should()
                .Be("https://media.marianne.net/sites/default/files/styles/mrn_article_large/public/sport-amateur-marianne-menace.jpg");
        }

        private static ExternalArticle SomeExternalArticle()
        {
            return new ExternalArticle(
                "someExternalId",
                "https://someurl",
                "some title",
                DateTimeOffset.Now,
                "some summary",
                "https://someImageUrl"
            );
        }
    }
}