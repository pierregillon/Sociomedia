using System.Collections.Generic;

namespace Sociomedia.Articles.Application.Projections
{
    public interface IProjectionLocator
    {
        IEnumerable<IProjection> FindProjections();
    }
}