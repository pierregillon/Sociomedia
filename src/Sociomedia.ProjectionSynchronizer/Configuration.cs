using System;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer
{
    public class Configuration
    {
        public EventStoreConfiguration EventStore { get; set; } = new EventStoreConfiguration();

        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new SqlDatabaseConfiguration();

        public ProjectionSynchronizationConfiguration ProjectionSynchronization { get; set; } = new ProjectionSynchronizationConfiguration();
    }

    public class EventStoreConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Uri Uri => new Uri($"tcp://{Login}:{Password}@{Server}:{Port}");
    }
}