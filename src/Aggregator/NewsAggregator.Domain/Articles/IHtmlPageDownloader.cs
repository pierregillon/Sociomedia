using System;
using System.Threading.Tasks;

namespace NewsAggregator.Domain.Articles {
    public interface IHtmlPageDownloader
    {
        Task<string> Download(Uri url);
    }
}