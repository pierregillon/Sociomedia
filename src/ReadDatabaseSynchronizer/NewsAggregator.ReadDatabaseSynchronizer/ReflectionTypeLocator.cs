using System;
using System.Linq;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    public class ReflectionTypeLocator : ITypeLocator
    {
        private static readonly Type[] Types = typeof(IDomainEvent).Assembly.GetTypes();

        public Type FindEventType(string typeName)
        {
            return Types.SingleOrDefault(x => x.Name == typeName);
        }
    }
}