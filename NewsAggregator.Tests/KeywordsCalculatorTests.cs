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
            var words = new[] { "test", "winter", "john", "mountains", "test", "summer", "winter", "test" };

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
        public void Keywords_can_be_a_combination_of_two_words()
        {
            var words = new[] { "john", "wick", "winter", "john", "wick", "summer" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new [] {
                    new Keyword("john wick", 2),
                    new Keyword("winter", 1),
                    new Keyword("summer", 1)
                });
        }

        [Fact]
        public void Keywords_can_be_a_combination_of_three_words()
        {
            var words = new[] { "john", "wick", "rocks", "winter", "john", "wick", "rocks", "summer" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                    new Keyword("john wick rocks", 2),
                    new Keyword("winter", 1),
                    new Keyword("summer", 1)
                });
        }

        [Fact]
        public void Keywords_can_be_a_combination_four_words()
        {
            var words = new[] { "john", "wick", "rocks", "now", "winter", "john", "wick", "rocks", "now", "summer" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                    new Keyword("john wick rocks now", 2),
                    new Keyword("winter", 1),
                    new Keyword("summer", 1)
                });
        }
    }
}