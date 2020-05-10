using System;
using System.Linq;
using Sociomedia.Domain.Medias;

namespace Sociomedia.Infrastructure
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