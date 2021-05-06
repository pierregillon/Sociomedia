namespace Sociomedia.Core.Infrastructure.EventStoring {
    public class EventStoreConfiguration
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 2113;
        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "changeit";

        public string ConnectionString => $"esdb://{Login}:{Password}@{Server}:{Port}?tls=false";
    }
}