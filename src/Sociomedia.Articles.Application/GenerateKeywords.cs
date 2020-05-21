using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using EventStore.ClientAPI;
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
        private readonly ILogger _logger;

        public GenerateKeywords(IRepository repository, KeywordsParser keywordsParser, IWebPageDownloader webPageDownloader, IHtmlParser htmlParser, ILogger logger)
        {
            _repository = repository;
            _keywordsParser = keywordsParser;
            _webPageDownloader = webPageDownloader;
            _htmlParser = htmlParser;
            _logger = logger;
        }

        public async Task On(ArticleImported @event)
        {
            try {
                var article = await _repository.Get<Article>(@event.Id);

                var articleContent = await GetArticleContent(article.Url);

                var concatenatedText = string.Join(" ", articleContent, article.Title, article.Summary);

                var keywords = _keywordsParser.Parse(concatenatedText).Take(50).ToArray();

                article.DefineKeywords(keywords);

                await _repository.Save(article);
            }
            catch (UnreachableWebDocumentException) { }
        }

        private async Task<string> GetArticleContent(string url)
        {
            try {
                return await url
                    .Pipe(_webPageDownloader.Download)
                    .Pipe(_htmlParser.ExtractPlainTextArticleContent);
            }
            catch (UnreachableWebDocumentException) {
                _logger.Error($"[{GetType().Name.SeparatePascalCaseWords().ToUpper()}] Unable to update keywords of the article {url} : unreachable web document.");
                throw;
            }
        }
    }
}