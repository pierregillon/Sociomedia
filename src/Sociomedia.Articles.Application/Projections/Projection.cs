using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Articles.Domain;

namespace Sociomedia.Articles.Application.Projections
{
    public abstract class Projection<T> : IProjection
    {
        private readonly InMemoryDatabase _database;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, MethodInfo> _onMethods;

        protected Projection(InMemoryDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;

            _onMethods = GetType()
                .GetMethods()
                .Where(x => x.Name == nameof(IProjection.On))
                .ToDictionary(x => x.GetParameters()[0].ParameterType);
        }

        protected Task<IReadOnlyCollection<T>> GetAll()
        {
            return Task.FromResult(_database.List<T>());
        }

        protected Task Add(T element)
        {
            _database.Add(element);
            return Task.CompletedTask;
        }

        protected Task Remove(T element)
        {
            _database.Remove(element);
            return Task.CompletedTask;
        }

        protected void LogError(string message)
        {
            _logger.Error($"[{GetType().Name.SeparatePascalCaseWords().ToUpper()}] {message}");
        }

        async Task IProjection.On(IEvent @event)
        {
            if (_onMethods.TryGetValue(@event.GetType(), out var onMethod)) {
                await (Task) onMethod.Invoke(this, new object[] { @event });
            }
            else {
                LogError("Method On() not found !");
            }
        }
    }
}