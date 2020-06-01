using FluentAssertions;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests {
    public class KeywordTests
    {
        [Fact]
        public void Two_keywords_with_different_occurence_but_same_word_are_equals()
        {
            new Keyword2("Test", 10)
                .Should()
                .Be(new Keyword2("Test", 11));
        }
    }
}