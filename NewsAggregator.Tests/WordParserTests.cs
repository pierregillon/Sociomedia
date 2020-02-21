using FluentAssertions;
using Xunit;

namespace NewsAggregator.Tests
{
    public class WordParserTests
    {
        private readonly WordParser _wordParser;

        public WordParserTests()
        {
            _wordParser = new WordParser();
        }

        [Fact]
        public void Convert_simple_html()
        {
            const string simpleHtml = "<html>hello world</html>";

            var words = _wordParser.ParseHtml(simpleHtml);

            words.Should().BeEquivalentTo("hello", "world");
        }

        [Fact]
        public void Convert_html_special_char()
        {
            const string simpleHtml = "<html>test&nbsp;test</html>";

            var words = _wordParser.ParseHtml(simpleHtml);

            words.Should().BeEquivalentTo("test", "test");
        }

        [Fact]
        public void Convert_html_special_char_other()
        {
            const string simpleHtml = "<html>d&eacute;couvert</html>";

            var words = _wordParser.ParseHtml(simpleHtml);

            words.Should().BeEquivalentTo("découvert");
        }

        [Fact]
        public void Do_not_parse_alt_attribute()
        {
            const string complexHtml = @"
                <article class=""article__content old__article-content - single"">
                    <figure>
                        <picture class=""article__media"">
                            <source srcset=""https://test.fr/2020/02/21/0/0/3840/4800/688/0/60/0/4f6b640_11cCi-RTjM8m8fgFfDTeF0GY.JPG"" media=""(min-width: 992px)"">
                            <img src=""https://img.lemde.fr/2020/02/21/0/0/3840/4800/688/0/60/0/4f6b640_11cCi-RTjM8m8fgFfDTeF0GY.JPG"" alt=""Simple image title"" class=""initial loaded"" data-was-processed=""true"">
                        </picture>
                    </figure>
                </article>";

            var words = _wordParser.ParseHtml(complexHtml);

            words.Should().BeEmpty();
        }

        [Fact]
        public void Parse_article_content()
        {
            const string complexHtml = @"
                <article class=""article__content old__article-content - single"">
                    <figure>
                        <picture class=""article__media"">
                            <source srcset=""https://test.fr/2020/02/21/0/0/3840/4800/688/0/60/0/4f6b640_11cCi-RTjM8m8fgFfDTeF0GY.JPG"" media=""(min-width: 992px)"">
                            <img src=""https://img.lemde.fr/2020/02/21/0/0/3840/4800/688/0/60/0/4f6b640_11cCi-RTjM8m8fgFfDTeF0GY.JPG"" alt=""Simple image title"" class=""initial loaded"" data-was-processed=""true"">
                        </picture>
                        <figcaption class=""article__legend"" aria-hidden=""“true“"">
                            Simple figure caption
                        </figcaption>
                    </figure>
                    <p class=""article__paragraph "">
                        Simple article content
                    </p>
                </article>";

            var words = _wordParser.ParseHtml(complexHtml);

            words.Should().BeEquivalentTo("Simple", "figure", "caption", "Simple", "article", "content");
        }
    }
}