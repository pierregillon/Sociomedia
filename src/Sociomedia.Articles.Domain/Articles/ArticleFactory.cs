﻿using System;
using System.Threading.Tasks;
using FluentAsync;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Articles.Domain.Keywords;

namespace Sociomedia.Articles.Domain.Articles
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly KeywordsParser _keywordsParser;

        public ArticleFactory(IHtmlParser htmlParser, IWebPageDownloader webPageDownloader, KeywordsParser keywordsParser)
        {
            _htmlParser = htmlParser;
            _webPageDownloader = webPageDownloader;
            _keywordsParser = keywordsParser;
        }

        public async Task<Article> Build(Guid mediaId, FeedItem feedItem)
        {
            feedItem.ImageUrl ??= await _webPageDownloader.Download(feedItem.Link)
                .PipeAsync(_htmlParser.ExtractArticleImageUrl)
                .PipeAsync(imageUrl => InjectHostIfMissing(imageUrl, feedItem.Link))
                .PipeAsync(UrlSanitizer.Sanitize);

            return new Article(mediaId, feedItem);
        }

        private static string InjectHostIfMissing(string imageUrl, string articleUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) {
                return imageUrl;
            }
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var absoluteImageUrl)) {
                return absoluteImageUrl.AbsoluteUri;
            }
            var articleUri = new Uri(articleUrl);
            var schemeAndHost = new Uri($"{articleUri.Scheme}://{articleUri.Host}");
            return new Uri(schemeAndHost, imageUrl).AbsoluteUri;
        }
    }
}