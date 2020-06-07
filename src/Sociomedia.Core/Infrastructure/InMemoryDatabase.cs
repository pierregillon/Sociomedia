using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Core.Infrastructure
{
    public class InMemoryDatabase
    {
        private readonly IDictionary<Type, List<object>> _store = new ConcurrentDictionary<Type, List<object>>();

        public void Add<T>(T item)
        {
            lock (_store) {
                if (_store.TryGetValue(typeof(T), out var results)) {
                    results.Add(item);
                }
                else {
                    _store.Add(typeof(T), new List<object> { item });
                }
            }
        }

        public IReadOnlyCollection<T> List<T>()
        {
            lock (_store) {
                if (_store.TryGetValue(typeof(T), out var results)) {
                    return results.Cast<T>().ToArray();
                }
                else {
                    return Array.Empty<T>();
                }
            }
        }

        public void Remove<T>(T element)
        {
            lock (_store) {
                if (_store.TryGetValue(typeof(T), out var results)) {
                    results.Remove(element);
                    return;
                }
            }
            throw new InvalidOperationException($"No list in memory found of the type {typeof(T).Name}");
        }
    }
}