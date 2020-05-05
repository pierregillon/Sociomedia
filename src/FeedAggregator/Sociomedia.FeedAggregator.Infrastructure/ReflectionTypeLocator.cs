using System;
using System.Linq;
using Sociomedia.FeedAggregator.Domain;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class ReflectionTypeLocator : ITypeLocator
    {
        private static readonly Type[] Types = typeof(IDomainEvent).Assembly.GetTypes();

        public Type FindEventType(string typeName)
        {
            return Types.SingleOrDefault(x => x.Name == typeName);
        }
    }
}