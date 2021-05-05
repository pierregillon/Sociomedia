using System.Collections.Generic;
using Sociomedia.Core.Application.Projections;
using StructureMap;

namespace Sociomedia.Core.Infrastructure
{
    public class ProjectionLocator : IProjectionLocator
    {
        private readonly IContainer _container;

        public ProjectionLocator(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<IProjection> FindProjections()
        {
            return _container.GetAllInstances<IProjection>();
        }
    }
}