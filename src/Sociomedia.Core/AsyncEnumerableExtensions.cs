using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.Core
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IReadOnlyCollection<T>> EnumerateAsync<T>(this IAsyncEnumerable<T> enumerable)
        {
            var list = new List<T>();
            await foreach (var item in enumerable) {
                list.Add(item);
            }
            return list;
        }
    }
}