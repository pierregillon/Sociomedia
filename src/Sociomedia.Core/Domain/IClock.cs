using System;

namespace Sociomedia.Core.Domain {
    public interface IClock
    {
        DateTimeOffset Now();
    }
}