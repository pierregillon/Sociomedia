using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sociomedia.Articles.Infrastructure;

namespace Sociomedia.FeedAggregator.Application
{
    public class FeedAggregatorApplication
    {
        private readonly ILogger _logger;
        private readonly FrenchKeywordDictionary _dictionary;
        private readonly Aggregator _aggregator;

        public FeedAggregatorApplication(ILogger logger, FrenchKeywordDictionary dictionary, Aggregator aggregator)
        {
            _logger = logger;
            _dictionary = dictionary;
            _aggregator = aggregator;
        }

        public async Task Run()
        {
            var source = new CancellationTokenSource();

            Info("Loading language dictionary ...");

            _dictionary.BuildFromFiles();

            await _aggregator.StartAggregation(source.Token);

            WaitForExit();

            Info("Stopping");

            source.Cancel();

            Info("Stopped");
        }

        private void Info(string message)
        {
            _logger.LogInformation("[APPLICATION] " + message);
        }

        private static void WaitForExit()
        {
            var exitEvent = new ManualResetEvent(false);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => { exitEvent.Set(); };

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
        }
    }
}