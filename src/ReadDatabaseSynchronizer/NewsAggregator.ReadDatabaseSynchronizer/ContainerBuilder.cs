using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using NewsAggregator.ReadDatabaseSynchronizer.ReadModels;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    public class ContainerBuilder
    {
        public static IContainer Build()
        {
            return new Container(configuration => {
                configuration.For<EventStoreOrg>().Singleton();
                configuration.For<IEventPublisher>().Use<StructureMapEventPublisher>();
                configuration.For<ITypeLocator>().Use<ReflectionTypeLocator>();
                configuration.For<ILogger>().Use<ConsoleLogger>();
                configuration.For<DbConnectionReadModel>().Singleton();
                configuration.For<IEventAcknowledger>().Use<EventAcknowledger>();

                configuration.Scan(scanner => {
                    scanner.TheCallingAssembly();
                    scanner.Convention<AllInterfacesConvention>();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                });
            });
        }

        public class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry registry)
            {
                foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                    foreach (var @interface in type.GetInterfaces()) {
                        registry.For(@interface).Use(type);
                    }
                }
            }
        }
    }
}