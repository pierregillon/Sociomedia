using System.Collections.Generic;

namespace Sociomedia.Core.Application.Projections
{
    public interface IProjectionLocator
    {
        IEnumerable<IProjection> FindProjections();
    }
}