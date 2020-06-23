using System;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Infrastructure {
    public class Clock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}