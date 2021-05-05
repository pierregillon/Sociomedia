using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class ReflectionTypeLocator<T> : ITypeLocator
    {
        private static readonly IDictionary<string, Type> Types = typeof(T)
            .Assembly
            .GetTypes()
            .Where(x => x.IsDomainEvent())
            .ToDictionary(x => x.Name);

        public Type FindEventType(string typeName)
        {
            if (Types.TryGetValue(typeName, out var type)) {
                return type;
            }
            return null;
        }
    }
}