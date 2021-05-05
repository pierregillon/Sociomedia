using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Core.Infrastructure;

namespace Sociomedia.Core.Application.Projections
{
    public abstract class Projection<T> : IProjection
    {
        private readonly InMemoryDatabase _database;
        private readonly ILogger _logger;

        protected Projection(InMemoryDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
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
    }
}