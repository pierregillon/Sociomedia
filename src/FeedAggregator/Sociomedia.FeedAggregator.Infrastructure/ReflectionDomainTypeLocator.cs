using System;
using System.Linq;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class ReflectionDomainTypeLocator : ITypeLocator
    {
        private static readonly Type[] Types = typeof(Media).Assembly.GetTypes();

        public Type FindEventType(string typeName)
        {
            return Types.SingleOrDefault(x => x.Name == typeName);
        }
    }
}