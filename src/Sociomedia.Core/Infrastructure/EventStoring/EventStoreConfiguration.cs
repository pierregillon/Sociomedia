using System;

namespace Sociomedia.Application.Infrastructure.EventStoring {
    public class EventStoreConfiguration
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 1113;
        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "changeit";

        public Uri Uri => new Uri($"tcp://{Login}:{Password}@{Server}:{Port}");
    }
}