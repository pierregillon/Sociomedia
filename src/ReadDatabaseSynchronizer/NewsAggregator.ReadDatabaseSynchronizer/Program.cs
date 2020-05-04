using System;
using System.Threading.Tasks;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = ContainerBuilder.Build();

            var eventStore = container.GetInstance<EventStoreOrg>();

            await eventStore.Connect("localhost");

            eventStore.StartListeningEvents(null);

            Console.WriteLine("Synchronization started.");
            do {
                Console.Write("Stop it ? => ");
            } while (Console.ReadLine() != "y");
            Console.WriteLine("Stopping...");
            eventStore.StopListeningEvents();
            Console.WriteLine("Stopped.");
        }
    }
}