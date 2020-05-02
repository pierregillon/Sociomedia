using System.Threading.Tasks;

namespace NewsAggregator.Infrastructure.Logging {
    public interface ILogger
    {
        Task LogInformation(string message, long? elapsedMilliseconds = null);
    }
}