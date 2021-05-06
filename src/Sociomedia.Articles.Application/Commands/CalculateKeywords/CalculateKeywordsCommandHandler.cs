using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Microsoft.Extensions.Logging;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Core;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Application.Commands.CalculateKeywords
{
    public class CalculateKeywordsCommandHandler : ICommandHandler<CalculateKeywordsCommand>
    {
        private readonly IRepository _repository;
        private readonly KeywordsParser _keywordsParser;
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly IHtmlParser _htmlParser;
        private readonly ILogger _logger;

        public CalculateKeywordsCommandHandler(IRepository repository, KeywordsParser keywordsParser, IWebPageDownloader webPageDownloader, IHtmlParser htmlParser, ILogger logger)
        {
            _repository = repository;
            _keywordsParser = keywordsParser;
            _webPageDownloader = webPageDownloader;
            _htmlParser = htmlParser;
            _logger = logger;
        }

        public async Task Handle(CalculateKeywordsCommand command)
        {
            try {
                var article = await _repository.Get<Article>(command.ArticleId);

                var keywords = await CalculateKeywords(article);

                article.DefineKeywords(keywords);

                await _repository.Save(article);
            }
            catch (UnreachableWebDocumentException) { }
        }

        private async Task<Keyword[]> CalculateKeywords(Article article)
        {
            var articleContent = await DownloadArticleContent(article.Url);

            var concatenatedText = string.Join(" ", articleContent, article.Title, article.Summary);

            return _keywordsParser.Parse(concatenatedText).Take(50).ToArray();
        }

        private async Task<string> DownloadArticleContent(string url)
        {
            try {
                return await url
                    .Pipe(_webPageDownloader.Download)
                    .Pipe(_htmlParser.ExtractPlainTextArticleContent);
            }
            catch (UnreachableWebDocumentException) {
                _logger.LogError($"[{GetType().Name.SeparatePascalCaseWords().ToUpper()}] Unable to update keywords of the article {url} : unreachable web document.");
                throw;
            }
        }
    }
}