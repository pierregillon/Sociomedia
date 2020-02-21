using FluentAssertions;
using Xunit;

namespace NewsAggregator.Tests
{
    public class KeywordsCalculatorTests
    {
        private readonly KeywordsCalculator _keywordsCalculator;

        public KeywordsCalculatorTests()
        {
            _keywordsCalculator = new KeywordsCalculator();
        }

        [Fact]
        public void Keywords_are_words_with_more_than_3_letters()
        {
            var words = new[] { "test", "a", "b", "c", "de", "abc" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("test", 1));
        }

        [Fact]
        public void Keywords_are_words_with_the_most_occurence()
        {
            var words = new[] { "test", "winter", "john", "mountains", "test", "summer", "test", "winter" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                        new Keyword("test", 3),
                        new Keyword("winter", 2),
                        new Keyword("john", 1),
                        new Keyword("mountains", 1),
                        new Keyword("summer", 1)
                    }
                );
        }

        [Fact]
        public void Keywords_are_words_combination()
        {
            var words = new[] { "a", "b", "a", "b" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("a b", 2));
        }
    }
}