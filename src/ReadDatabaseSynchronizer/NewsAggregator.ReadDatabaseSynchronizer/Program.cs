using System;
using System.Threading.Tasks;
using LinqToDB.Data;
using NewsAggregator.ReadDatabaseSynchronizer.ReadModels;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DataConnection.DefaultSettings = new MySettings();

            var container = ContainerBuilder.Build();

            var synchronizer = container.GetInstance<DomainEventSynchronizer>();

            await synchronizer.StartSynchronization();

            Console.WriteLine("Synchronization started.");
            do {
                Console.Write("Stop it ? => ");
            } while (Console.ReadLine() != "y");
            Console.WriteLine("Stopping...");
            synchronizer.StopSynchronization();
            Console.WriteLine("Stopped.");
        }
    }
}