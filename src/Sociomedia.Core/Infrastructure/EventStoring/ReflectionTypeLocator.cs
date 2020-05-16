using System;
using System.Linq;
using System.Reflection;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public class ReflectionTypeLocator : ITypeLocator
    {
        private static readonly Type[] Types = Assembly.GetExecutingAssembly().GetTypes();

        public Type FindEventType(string typeName)
        {
            return Types.SingleOrDefault(x => x.Name == typeName);
        }
    }
}