using System;
using System.IO;
using Newtonsoft.Json;

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
}