using System;
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

                var synchronizer = container.GetInstance<DomainEventSynchronizer>();

                container.GetInstance<ILogger>().Debug("Read database synchronizer started.");
            
                await synchronizer.StartSynchronization();

                Console.ReadKey();

                synchronizer.StopSynchronization();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}