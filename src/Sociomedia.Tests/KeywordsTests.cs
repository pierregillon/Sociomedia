using FluentAssertions;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests
{
    public class KeywordsTests
    {
        [Fact]
        public void Two_keywords_with_same_elements_are_equals_ignoring_order()
        {
            var keywords1 = new Keywords(new[] { "test", "test2" });
            var keywords2 = new Keywords(new[] { "test2", "test" });

            keywords1.Should().Be(keywords2);
            keywords1.SequenceEqual(keywords2).Should().BeTrue();
        }

        [Fact]
        public void Keywords_contains_all()
        {
            var keywords1 = new Keywords(new[] { "toto", "test2", "test" });
            var keywords2 = new Keywords(new[] { "test", "test2" });

            keywords1.ContainsAll(keywords2).Should().BeTrue();
        }

        [Fact]
        public void Keywords_intersection()
        {
            var keywords1 = new Keywords(new[] { "toto", "test2", "test" });

            keywords1
                .Intersect(new Keywords(new[] { "test", "test2", "titi"}))
                .Should()
                .Be(new Keywords(new[] { "test", "test2" }));
        }

        [Fact]
        public void Keywords_intersection_is_ordered()
        {
            new Keywords(new[] { "a", "b", "c" })
                .Intersect(new Keywords(new[] { "c", "b", "e" }))
                .Should()
                .Be(new Keywords(new[] { "b", "c" }));
        }
    }
}