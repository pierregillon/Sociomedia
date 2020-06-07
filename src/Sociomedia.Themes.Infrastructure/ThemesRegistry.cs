using CQRSlite.Domain;
using CQRSlite.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Core.Infrastructure.Logging;
using Sociomedia.Themes.Application;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.Themes.Infrastructure
{
    public class ThemesRegistry : Registry
    {
        public ThemesRegistry()
        {
            Scan(scanner => {
                scanner.Assembly("Sociomedia.Themes.Application");
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<ThemeProjection>().Singleton();
        }

        private class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry services)
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