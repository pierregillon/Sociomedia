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
        public void Keywords_are_words_with_more_than_2_letters()
        {
            var words = new[] { "test", "a", "a", "a", "test" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("test", 2));
        }

        [Fact]
        public void Keywords_are_ordered_by_occurrence()
        {
            var words = new[] { "test", "winter", "john", "a", "winter", "test", "john", "b", "test","c", "winter", "d", "test" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                        new Keyword("test", 4),
                        new Keyword("winter", 3),
                        new Keyword("john", 2),
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
                .BeEquivalentTo(new Keyword("john wick", 2));
        }

        [Fact]
        public void Keywords_can_be_a_combination_of_three_words()
        {
            var words = new[] { "john", "wick", "rocks", "winter", "john", "wick", "rocks" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("john wick rocks", 2));
        }

        [Fact]
        public void Keywords_can_be_a_combination_four_words()
        {
            var words = new[] { "john", "wick", "rocks", "now", "winter", "time", "john", "wick", "rocks", "now", "summer", "winter", "time" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("john wick rocks now", 2), new Keyword("winter time", 2));
        }

        [Fact]
        public void Keywords_combinations_have_always_unique_words()
        {
            var words = new[] { "john", "wick", "rocks", "now", "winter", "time", "john", "wick", "rocks", "now", "summer", "winter", "time" };

            var keywords = _keywordsCalculator.Calculate(words, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                    new Keyword("john wick rocks now", 2),
                    new Keyword("winter time", 2)
                });
        }

        [Fact]
        public void Keywords_group_words_ignoring_case_or_diacritics()
        {
            var words = new[] { "TEST", "a", "Test", "b", "test", "c", "tést" };

            var keywords = _keywordsCalculator.Calculate(words, 1);

            keywords
                .Should()
                .BeEquivalentTo(new Keyword("test", 4));
        }
    }
}