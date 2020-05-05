using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public class InMemoryDatabase
    {
        private readonly IDictionary<Type, List<object>> _store = new Dictionary<Type, List<object>>();

        public void Add<T>(T item)
        {
            if (_store.TryGetValue(typeof(T), out var results)) {
                results.Add(item);
            }
            else {
                _store.Add(typeof(T), new List<object> { item });
            }
        }

        public IReadOnlyCollection<T> List<T>()
        {
            if (_store.TryGetValue(typeof(T), out var results)) {
                return results.Cast<T>().ToList();
            }
            else {
                return Array.Empty<T>();
            }
        }
    }
}