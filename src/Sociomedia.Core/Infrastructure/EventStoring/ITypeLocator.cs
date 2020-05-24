using System;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public interface ITypeLocator
    {
        Type FindEventType(string typeName);
    }
}