using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;

namespace Sociomedia.Articles.Infrastructure
{
    public class ReflectionTypeLocator : ITypeLocator
    {
        private static readonly IDictionary<string, Type> Types = typeof(ArticleEvent)
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