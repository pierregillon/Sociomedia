using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Themes.Domain;
using Sociomedia.Themes.Infrastructure;
using StructureMap;

namespace Sociomedia.ThemeCalculator
{
    public class ContainerBuilder
    {
        public static Container Build(Configuration configuration)
        {
            return new Container(registry => {
                registry.IncludeRegistry(new CoreRegistry(configuration.EventStore));
                registry.IncludeRegistry<ThemesRegistry>();
                registry.For<Calculator>().Singleton();
                registry.For<ITypeLocator>().Use<ReflectionTypeLocator<ThemeEvent>>().Singleton();
                registry.For<IEventBus>().Use<EventStoreOrgBus>().Singleton();
                registry.For<IEventPositionRepository>().Use<EventPositionRepository>();
                registry.For<Configuration>().Use(configuration).Singleton();
            });
        }
    }
}