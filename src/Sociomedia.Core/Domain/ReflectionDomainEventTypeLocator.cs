using System;
using System.Linq;

namespace Sociomedia.Core.Domain
{
    public class ReflectionDomainEventTypeLocator : IDomainEventTypeLocator
    {
        private static readonly Type[] Types = typeof(IDomainEventTypeLocator).Assembly.GetTypes();

        public Type FindEventType(string typeName)
        {
            return Types.SingleOrDefault(x => x.Name == typeName);
        }
    }
}