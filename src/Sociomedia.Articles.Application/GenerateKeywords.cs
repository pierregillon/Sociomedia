using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Application
{
    public class GenerateKeywords : IEventListener<ArticleImported>
    {
        private readonly IRepository _repository;
        private readonly KeywordsParser _keywordsParser;
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly IHtmlParser _htmlParser;

        public GenerateKeywords(IRepository repository, KeywordsParser keywordsParser, IWebPageDownloader webPageDownloader, IHtmlParser htmlParser)
        {
            _repository = repository;
            _keywordsParser = keywordsParser;
            _webPageDownloader = webPageDownloader;
            _htmlParser = htmlParser;
        }

        public async Task On(ArticleImported @event)
        {
            var article = await _repository.Get<Article>(@event.Id);

            var articleContent = await article.Url
                .Pipe(_webPageDownloader.Download)
                .Pipe(_htmlParser.ExtractPlainTextArticleContent);

            var concatenatedText = string.Join(" ", articleContent, article.Title, article.Summary);

            var keywords = _keywordsParser.Parse(concatenatedText).Take(50).ToArray();

            article.DefineKeywords(keywords);

            await _repository.Save(article);
        }
    }
}