using System;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Domain.Articles {
    public interface IHtmlPageDownloader
    {
        Task<string> Download(Uri url);
    }
}