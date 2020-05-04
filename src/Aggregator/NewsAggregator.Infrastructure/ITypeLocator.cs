using System;

namespace NewsAggregator.Infrastructure
{
    public interface ITypeLocator
    {
        Type FindEventType(string typeName);
    }
}