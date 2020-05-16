using System.IO;
using System.Threading.Tasks;

namespace Sociomedia.Articles.Domain
{
    public interface IWebPageDownloader
    {
        Task<string> Download(string url);
        Task<Stream> DownloadStream(string url);
    }
}