using System.Linq;
using FluentAssertions;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests.AcceptanceTests
{
    public class KeywordsIntersectionTests
    {
        [Fact]
        public void Two_same_intersections_are_equals()
        {
            var firstIntersection = new Keywords(new[] { "test" });
            var secondIntersection = new Keywords(new[] { "test" });

            firstIntersection.Should().Be(secondIntersection);

            new[] { firstIntersection, secondIntersection }.GroupBy(x => x).Should().HaveCount(1);
        }
    }
}