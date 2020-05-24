using System.Threading.Tasks;
using Sociomedia.Core.Application;

namespace Sociomedia.Core.Infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task Dispatch<T>(T command) where T : ICommand;
        Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>;
    }
}