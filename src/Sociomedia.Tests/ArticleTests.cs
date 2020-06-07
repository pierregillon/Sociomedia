using System;
using FluentAssertions;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests
{
    public class ArticleTests
    {
        [Fact]
        public void Two_articles_with_the_same_id_and_keywords_are_equals()
        {
            var id = Guid.NewGuid();

            var article1 = new Article(id, new[] { new Keyword("Test", 10), });
            var article2 = new Article(id, new[] { new Keyword("Test", 10), });

            article1.Should().Be(article2);
        }
    }
}