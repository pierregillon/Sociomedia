using System;
using System.IO;
using Newtonsoft.Json;
using Sociomedia.Infrastructure;

namespace Sociomedia.FeedAggregator
{
    public class ConfigurationManager<T>
    {
        private const string CONFIG_JSON = "./config.json";

        public static T Read()
        {
            if (!File.Exists(CONFIG_JSON)) {
                throw new InvalidOperationException("The configuration file 'config.json' cannot be found.");
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(CONFIG_JSON));
        }
    }

    public class Configuration
    {
        public EventStoreConfiguration EventStore { get; set; } = new EventStoreConfiguration();

        public FeedAggregatorConfiguration FeedAggregator { get; set; } = new FeedAggregatorConfiguration();
    }

    public class FeedAggregatorConfiguration
    {
        public string SynchronizationInterval { get; set; }

        public TimeSpan SynchronizationTimespan => TimeSpan.TryParse(SynchronizationInterval, out var result)
            ? result
            : throw new Exception("Unable to parse SynchronizationInterval from configuration.");
    }
}