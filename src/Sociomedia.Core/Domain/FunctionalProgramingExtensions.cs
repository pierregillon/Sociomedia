using System;
using System.Threading.Tasks;

namespace Sociomedia.Application.Domain
{
    public static class FunctionalProgramingExtensions
    {
        public static TResult Pipe<T, TResult>(this T element, Func<T, TResult> action)
        {
            return action(element);
        }

        public static async Task<TResult> Pipe<T, TResult>(this Task<T> element, Func<T, TResult> action)
        {
            return action(await element);
        }

        public static async Task<TResult> Pipe<T, TResult>(this Task<T> element, Func<T, Task<TResult>> action)
        {
            return await action(await element);
        }
    }
}