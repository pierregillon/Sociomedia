using FluentAssertions.Equivalency;
using Sociomedia.DomainEvents;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public static class EquivalencyAssertionOptionsExtensions
    {
        public static EquivalencyAssertionOptions<DomainEvent> ExcludeDomainEventTechnicalFields(this EquivalencyAssertionOptions<DomainEvent> options)
        {
            return options
                .IncludingAllRuntimeProperties()
                .Excluding(a => a.Id)
                .Excluding(a => a.Version)
                .Excluding(a => a.TimeStamp);
        }
    }
}