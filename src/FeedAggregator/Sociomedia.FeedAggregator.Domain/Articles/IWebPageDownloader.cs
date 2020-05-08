using System;
using System.IO;
using System.Threading.Tasks;

namespace Sociomedia.Domain.Articles
{
    public interface IWebPageDownloader
    {
        Task<string> Download(Uri url);
        Task<Stream> DownloadStream(Uri url);
    }
}