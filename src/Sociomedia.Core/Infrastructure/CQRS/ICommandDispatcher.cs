using System.Threading.Tasks;
using Sociomedia.Application.Application;

namespace Sociomedia.Application.Infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
        Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>;
    }
}