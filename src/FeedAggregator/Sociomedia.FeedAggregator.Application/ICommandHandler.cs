using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application {
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command);
    }
}