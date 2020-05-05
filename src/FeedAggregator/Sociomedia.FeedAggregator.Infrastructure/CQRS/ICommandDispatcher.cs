using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Application;

namespace Sociomedia.FeedAggregator.Infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
    }
}