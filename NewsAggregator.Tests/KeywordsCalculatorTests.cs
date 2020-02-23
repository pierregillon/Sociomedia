using FluentAssertions;
using Xunit;

namespace NewsAggregator.Tests
{
    public class KeywordsCalculatorTests
    {
        private readonly KeywordsParser _keywordsParser;

        public KeywordsCalculatorTests()
        {
            _keywordsParser = new KeywordsParser();
        }

        [Theory]
        [InlineData("test a ad ax test")]
        [InlineData("a bc c test a test")]
        public void Keywords_are_words_with_more_than_2_letters(string text)
        {
            _keywordsParser.Parse(text, 10)
                .Should()
                .BeEquivalentTo(new Keyword("test", 2));
        }

        [Fact]
        public void Keywords_are_ordered_by_occurrence()
        {
            const string text = "c winter d test test winter john a winter test john b test";

            var keywords = _keywordsParser.Parse(text, 10);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                        new Keyword("test", 4),
                        new Keyword("winter", 3),
                        new Keyword("john", 2),
                    }
                );
        }

        [Theory]
        [InlineData("john wick winter john wick summer")]
        [InlineData("john wick is a real john wick")]
        public void Keywords_can_be_a_combination_of_two_words(string text)
        {
            _keywordsParser.Parse(text, 10)
                .Should()
                .ContainEquivalentOf(new Keyword("john wick", 2));
        }

        [Theory]
        [InlineData("I said to you, john wick rocks ! Yes, john wick rocks.")]
        [InlineData("john wick rocks winter john wick rocks")]
        public void Keywords_can_be_a_combination_of_three_words(string text)
        {
            _keywordsParser.Parse(text, 10)
                .Should()
                .ContainEquivalentOf(new Keyword("john wick rocks", 2));
        }

        [Theory]
        [InlineData("John wick enjoys killing during winter time. Of course, john wick enjoys killing in summer time too.")]
        public void Keywords_can_be_a_combination_four_words(string text)
        {
            _keywordsParser.Parse(text, 10)
                .Should()
                .ContainEquivalentOf(new Keyword("john wick enjoys killing", 2));
        }

        [Theory]
        [InlineData("TEST a Test b test c tést")]
        public void Keyword_is_a_group_of_words_ignoring_case_or_diacritics(string text)
        {
            _keywordsParser.Parse(text, 1)
                .Should()
                .BeEquivalentTo(new Keyword("test", 4));
        }
    }
}