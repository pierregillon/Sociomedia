using System;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Infrastructure.Logging {
    public interface ILogger
    {
        Task LogInformation(string message, long? elapsedMilliseconds = null);
        Task LogError(Exception ex);
    }
}