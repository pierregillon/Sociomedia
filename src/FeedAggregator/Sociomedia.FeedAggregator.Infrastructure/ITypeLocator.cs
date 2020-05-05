using System;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public interface ITypeLocator
    {
        Type FindEventType(string typeName);
    }
}