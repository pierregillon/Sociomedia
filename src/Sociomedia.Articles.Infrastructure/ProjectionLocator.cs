using System.Collections.Generic;
using Sociomedia.Articles.Application.Projections;
using StructureMap;

namespace Sociomedia.Articles.Infrastructure
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