using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sociomedia.ThemeCalculator
{
    public class ThemeCalculatorApplication
    {
        private readonly ILogger _logger;
        private readonly Calculator _calculator;

        public ThemeCalculatorApplication(ILogger logger, Calculator calculator)
        {
            _logger = logger;
            _calculator = calculator;
        }

        public async Task Run()
        {
            Info("Starting theme calculator ...");

            var source = new CancellationTokenSource();

            await _calculator.StartCalculation(source.Token);

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