using System;
using System.Threading.Tasks;

namespace NewsAggregator.Infrastructure.Logging
{
    public class ConsoleLogger : ILogger
    {
        public Task LogInformation(string message, long? elapsedMilliseconds = null)
        {
            Console.WriteLine($"[{DateTimeOffset.Now}] [{elapsedMilliseconds?.ToString() ?? "-"} MS]\t{message}");
            return Task.CompletedTask;
        }

        public Task LogError(Exception ex)
        {
            Console.WriteLine($"[{DateTimeOffset.Now}] [ ERROR ]\t{ex}");
            return Task.CompletedTask;
        }
    }
}