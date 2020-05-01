using System.Threading.Tasks;
using NewsAggregator.Application;

namespace NewsAggregator.Infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
    }
}