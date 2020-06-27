using System;
using System.IO;
using Newtonsoft.Json;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Themes.Application;

namespace Sociomedia.ThemeCalculator
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

        public ThemeCalculatorConfiguration ThemeCalculator { get; set; } = new ThemeCalculatorConfiguration();
    }
}