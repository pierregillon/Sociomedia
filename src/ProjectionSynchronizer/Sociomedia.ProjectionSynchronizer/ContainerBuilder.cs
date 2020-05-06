using BaselineTypeDiscovery;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Lamar;
using Lamar.Scanning.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Sociomedia.DomainEvents;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ProjectionSynchronizer.Application.EventListeners;
using Sociomedia.ProjectionSynchronizer.Infrastructure;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer
{
    public class ContainerBuilder
    {
        public static IContainer Build(Configuration configuration)
        {
            return new Container(x => {
                x.For<IEventBus>().Use<EventStoreOrg>().Singleton();
                x.For<IEventPublisher>().Use<StructureMapEventPublisher>();
                x.For<IDomainEventTypeLocator>().Use<ReflectionDomainEventTypeLocator>().Singleton();
                x.For<ILogger>().Use<ConsoleLogger>();
                x.For<DbConnectionReadModel>().Use<DbConnectionReadModel>().Singleton();
                x.For<IStreamPositionRepository>().Use<StreamPositionRepository>();
                x.For<DbSettings>();

                x.Scan(scanner => {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                });

                x.For<EventStoreConfiguration>().Use(configuration.EventStore).Singleton();
                x.For<SqlDatabaseConfiguration>().Use(configuration.SqlDatabase).Singleton();
                x.For<ProjectionSynchronizationConfiguration>().Use(configuration.ProjectionSynchronization).Singleton();

                x.For<ArticleTableSynchronizer>().Use<ArticleTableSynchronizer>().Singleton();

                x.Injectable<IArticleRepository>();
                x.Injectable<IStreamPositionRepository>();
                x.Injectable<IEventBus>();
                x.Injectable<ILogger>();
            });
        }

        public class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, ServiceRegistry services)
            {
                foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                    foreach (var @interface in type.GetInterfaces()) {
                        services.For(@interface).Use(type);
                    }
                }
            }
        }
    }
}