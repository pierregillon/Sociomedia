using System;
using EventStore.ClientAPI;

namespace Sociomedia.FeedAggregator.Tests {
    public class EmptyLogger : ILogger
    {
        public void Error(string format, params object[] args) { }

        public void Error(Exception ex, string format, params object[] args) { }

        public void Info(string format, params object[] args) { }

        public void Info(Exception ex, string format, params object[] args) { }

        public void Debug(string format, params object[] args) { }

        public void Debug(Exception ex, string format, params object[] args) { }
    }
}