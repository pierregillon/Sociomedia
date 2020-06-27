using System.Linq;
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
        public void Keywords_does_not_contains_all()
        {
            var keywords1 = new Keywords(new[] { "toto", "test2", "test" });
            var keywords2 = new Keywords(new[] { "test", "test2", "tata" });

            keywords1.ContainsAll(keywords2).Should().BeFalse();
        }

        [Fact]
        public void Keywords_intersection()
        {
            var keywords1 = new Keywords(new[] { "toto", "test2", "test" });

            keywords1
                .Intersect(new Keywords(new[] { "test", "test2", "titi" }))
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


        [Theory]
        [InlineData("a")]
        [InlineData("a ; b")]
        [InlineData("a ; b ; c")]
        [InlineData("a ; b ; c ; d")]
        [InlineData("a ; b ; c ; d ; e")]
        public void Keywords_are_compatible_for_theme_creation_between_1_and_5(string values)
        {
            new Keywords(values.Split(";").Select(x => x.Trim()))
                .IsCompatibleForThemeCreation()
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("a ; b ; c ; d ; e ; f")]
        public void Keywords_are_not_compatible_over_6(string values)
        {
            new Keywords(values.Split(";").Select(x => x.Trim()))
                .IsCompatibleForThemeCreation()
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Keywords_are_not_compatible_if_none()
        {
            new Keywords(Enumerable.Empty<string>())
                .IsCompatibleForThemeCreation()
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Same_keywords_can_be_grouped()
        {
            var grouped = new Keywords[] {
                new Keywords(new []{"a", "b"}),
                new Keywords(new []{"a", "b"}),
            }.GroupBy(x => x);

            grouped.Should().HaveCount(1);
        }
    }
}