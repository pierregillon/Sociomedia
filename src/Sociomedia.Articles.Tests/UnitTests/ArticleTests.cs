using System;
using System.Linq;
using FluentAssertions;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Feeds;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class ArticleTests
    {
        [Fact]
        public void Updating_article()
        {
            var previousArticle = SomeExternalArticle();

            var article = new Article(Guid.NewGuid(), previousArticle);

            var newVersionOfExternalArticle = SomeOtherArticle();

            article.UpdateFromFeed(newVersionOfExternalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleUpdated>()
                .Should()
                .BeEquivalentTo(new[] {
                    new ArticleUpdated(
                        article.Id,
                        newVersionOfExternalArticle.Title,
                        newVersionOfExternalArticle.Summary,
                        newVersionOfExternalArticle.PublishDate.Value,
                        newVersionOfExternalArticle.Link,
                        newVersionOfExternalArticle.ImageUrl),
                });
        }

        [Fact]
        public void Updating_article_does_not_change_image_url_if_new_one_is_undefined()
        {
            var previousArticle = SomeExternalArticle();

            var article = new Article(Guid.NewGuid(), previousArticle);

            var newVersionOfExternalArticle = SomeExternalArticle();
            newVersionOfExternalArticle.ImageUrl = null;

            article.UpdateFromFeed(newVersionOfExternalArticle);

            article
                .GetUncommittedChanges()
                .OfType<ArticleUpdated>()
                .Single()
                .ImageUrl
                .Should()
                .Be(previousArticle.ImageUrl);
        }

        // ----- Private methods

        private static FeedItem SomeExternalArticle()
        {
            return new FeedItem {
                Id = "someExternalId",
                Link = "https://someurl",
                Title = "some title",
                PublishDate = DateTimeOffset.Now,
                Summary = "some summary",
                ImageUrl = "https://someImageUrl"
            };
        }

        private static FeedItem SomeOtherArticle()
        {
            return new FeedItem {
                Id = "someExternalId2",
                Link = "https://someurl2",
                Title = "some title2",
                PublishDate = DateTimeOffset.Now,
                Summary = "some summary2",
                ImageUrl = "https://someImageUrl2"
            };
        }
    }
}