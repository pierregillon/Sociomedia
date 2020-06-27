using System;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Application.Projections;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.Themes.Infrastructure
{
    public class ThemesRegistry : Registry
    {
        public ThemesRegistry(ThemeCalculatorConfiguration configuration)
        {
            Scan(scanner => {
                scanner.Assembly("Sociomedia.Themes.Application");
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<ThemeProjectionRepository>().Singleton();
            For<ThemeCalculatorConfiguration>().Use(x => configuration).Singleton();
            For<ThemeDataFinder>().Use<ThemeDataFinder>();
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