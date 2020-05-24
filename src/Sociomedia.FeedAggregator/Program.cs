using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Application;

namespace Sociomedia.FeedAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = ConfigurationManager<Configuration>.Read();

            var container = ContainerBuilder.Build(configuration);

            var application = container.GetInstance<FeedAggregatorApplication>();

            await application.Run();
        }
    }
}