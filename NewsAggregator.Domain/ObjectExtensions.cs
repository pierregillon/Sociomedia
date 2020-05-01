using System;

namespace NewsAggregator.Domain
{
    public static class ObjectExtensions
    {
        public static TResult Pipe<T, TResult>(this T element, Func<T, TResult> action)
        {
            return action(element);
        }
    }
}