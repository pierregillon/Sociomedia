using System;
using System.IO;
using System.Threading.Tasks;

namespace Sociomedia.Domain.Articles
{
    public interface IWebPageDownloader
    {
        Task<string> Download(string url);
        Task<Stream> DownloadStream(string url);
    }
}