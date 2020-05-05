using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using LinqToDB.Data;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ProjectionSynchronizer.Infrastructure.ReadModels;

namespace Sociomedia.ProjectionSynchronizer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = ContainerBuilder.Build();

            DataConnection.DefaultSettings = container.GetInstance<DbSettings>();

            var synchronizer = container.GetInstance<DomainEventSynchronizer>();

            container.GetInstance<ILogger>().Debug("Read database synchronizer started.");
            
            await synchronizer.StartSynchronization();

            Console.ReadKey();

            synchronizer.StopSynchronization();
        }
    }
}