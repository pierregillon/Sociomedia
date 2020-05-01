using System.Threading.Tasks;

namespace NewsAggregator.Application {
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command);
    }
}