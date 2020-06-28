using FluentAssertions;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests {
    public class KeywordTests
    {
        [Fact]
        public void Two_keywords_with_different_occurence_are_not_equals()
        {
            new Keyword("Test", 10)
                .Should()
                .NotBe(new Keyword("Test", 11));
        }

        [Fact]
        public void A_keyword_can_be_composed()
        {
            new Keyword("john wick", 1)
                .IsComposed
                .Should()
                .BeTrue();
        }
    }
}