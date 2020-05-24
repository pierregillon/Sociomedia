using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class AsyncList<T> : List<T>, IAsyncEnumerable<T>
    {
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var element in this) {
                yield return element;
            }

            await Task.CompletedTask;
        }
    }
}