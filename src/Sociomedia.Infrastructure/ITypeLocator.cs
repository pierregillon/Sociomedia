using System;

namespace Sociomedia.Infrastructure
{
    public interface ITypeLocator
    {
        Type FindEventType(string typeName);
    }
}