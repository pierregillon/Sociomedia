using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CQRSlite.Events;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Application.Projections
{
    public class ProjectionInfo
    {
        private readonly IProjection _projection;
        private readonly MethodInfo _on;

        public ProjectionInfo(IProjection projection, Type eventType)
        {
            _projection = projection;

            _on = projection
                .GetType()
                .GetMethods()
                .Where(x => x.Name == nameof(IEventListener<DomainEvent>.On))
                .Single(x => x.GetParameters()[0].ParameterType == eventType);
        }

        public Task On(IEvent @event)
        {
            return (Task) _on.Invoke(_projection, new object[] { @event });
        }
    }
}