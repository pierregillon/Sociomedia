using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.DomainEvents;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ProjectionSynchronizer.Infrastructure;
using Sociomedia.ReadModel.DataAccess;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.ProjectionSynchronizer
{
    public class ContainerBuilder
    {
        public static IContainer Build(Configuration configuration)
        {
            return new Container(x => {
                x.For<EventStoreOrg>().Singleton();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();
                x.For<IDomainEventTypeLocator>().Use<ReflectionDomainEventTypeLocator>().Singleton();
                x.For<ILogger>().Use<ConsoleLogger>();
                x.For<DbConnectionReadModel>().Singleton();
                x.For<IStreamPositionRepository>().Use<StreamPositionRepository>();
                x.For<DbSettings>();

                x.Scan(scanner => {
                    scanner.TheCallingAssembly();
                    scanner.Convention<AllInterfacesConvention>();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                });

                x.For<EventStoreConfiguration>().Use(configuration.EventStore).Singleton();
                x.For<SqlDatabaseConfiguration>().Use(configuration.SqlDatabase).Singleton();
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