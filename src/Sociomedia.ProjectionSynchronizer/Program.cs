using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using LinqToDB.Data;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try {
                var configuration = ConfigurationManager<Configuration>.Read();

                var container = ContainerBuilder.Build(configuration);

                DataConnection.DefaultSettings = container.GetInstance<DbSettings>();

                container.GetInstance<DbConnectionReadModel>().GenerateMissingTables();

                var synchronizer = container.GetInstance<DomainEventSynchronizer>();

                container.GetInstance<ILogger>().Debug("Read database synchronizer started.");

                await synchronizer.StartSynchronization();

                Console.WriteLine("Started");

                WaitForExit();

                synchronizer.StopSynchronization();

                Console.WriteLine("Stopped");
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void WaitForExit()
        {
            var exitEvent = new ManualResetEvent(false);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                exitEvent.Set();
            };

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
        }
    }
}