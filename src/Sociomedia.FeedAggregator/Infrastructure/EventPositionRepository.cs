using System;
using System.IO;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class EventPositionRepository : IEventPositionRepository
    {
        private const string FILE_NAME = ".StreamPosition";

        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);

        public async Task<long?> GetLastProcessedPosition()
        {
            if (File.Exists(FilePath)) {
                var fileContent = await File.ReadAllTextAsync(FilePath);
                return long.Parse(fileContent);
            }
            return null;
        }

        public async Task Save(long position)
        {
            await File.WriteAllTextAsync(FilePath, position.ToString());
        }
    }
}