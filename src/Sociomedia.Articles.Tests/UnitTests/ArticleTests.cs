using System;
using System.Linq;
using FluentAssertions;
using Sociomedia.Articles.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class ArticleTests
    {
        [Fact]
        public void Updating_article()
        {
            var previousArticle = SomeExternalArticle();

            var article = new Article(Guid.NewGuid(), previousArticle, new string[0]);

            var newVersionOfExternalArticle = SomeOtherArticle();

            article.Update(newVersionOfExternalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleUpdated>()
                .Should()
                .BeEquivalentTo(new[] {
                    new ArticleUpdated(
                        article.Id,
                        newVersionOfExternalArticle.Title,
                        newVersionOfExternalArticle.Summary,
                        newVersionOfExternalArticle.PublishDate,
                        newVersionOfExternalArticle.Url,
                        newVersionOfExternalArticle.ImageUrl),
                });
        }

        [Fact]
        public void Updating_article_does_not_change_image_url_if_new_one_is_undefined()
        {
            var previousArticle = SomeExternalArticle();

            var article = new Article(Guid.NewGuid(), previousArticle, new string[0]);

            var newVersionOfExternalArticle = SomeExternalArticle();
            newVersionOfExternalArticle.ImageUrl = null;

            article.Update(newVersionOfExternalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleUpdated>()
                .Single()
                .ImageUrl
                .Should()
                .Be(previousArticle.ImageUrl);
        }

        // ----- Private methods

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

        private static ExternalArticle SomeOtherArticle()
        {
            return new ExternalArticle(
                "someExternalId2",
                "https://someurl2",
                "some title2",
                DateTimeOffset.Now,
                "some summary2",
                "https://someImageUrl2"
            );
        }
    }
}