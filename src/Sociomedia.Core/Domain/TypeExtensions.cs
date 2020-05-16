using System;

namespace Sociomedia.Application.Domain
{
    public static class TypeExtensions
    {
        public static bool IsDomainEvent(this Type type)
        {
            return !type.IsAbstract && type.IsSubclassOf(typeof(DomainEvent));
        }
    }
}