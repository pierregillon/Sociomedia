using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using LinqToDB.Data;
using NewsAggregator.ReadDatabaseSynchronizer.Application;
using NewsAggregator.ReadDatabaseSynchronizer.Infrastructure.ReadModels;

namespace NewsAggregator.ReadDatabaseSynchronizer
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