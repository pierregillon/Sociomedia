using System.Threading.Tasks;

namespace Sociomedia.Core.Application {
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command);
    }

    public interface ICommandHandler<in T, TResult> where T : ICommand<TResult>
    {
        Task<TResult> Handle(T command);
    }
}