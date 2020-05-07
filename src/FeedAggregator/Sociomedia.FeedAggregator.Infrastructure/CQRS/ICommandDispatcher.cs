using System.Threading.Tasks;
using Sociomedia.Application;

namespace Sociomedia.Infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
        Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>;
    }
}