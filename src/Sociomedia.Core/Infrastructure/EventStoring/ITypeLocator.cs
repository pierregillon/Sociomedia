using System;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public interface ITypeLocator
    {
        Type FindEventType(string typeName);
    }
}