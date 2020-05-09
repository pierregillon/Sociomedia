using FluentAssertions.Equivalency;
using Sociomedia.Domain;

namespace FeedAggregator.Tests {
    public static class EquivalencyAssertionOptionsExtensions
    {
        public static EquivalencyAssertionOptions<DomainEvent> ExcludeDomainEventTechnicalFields(this EquivalencyAssertionOptions<DomainEvent> options)
        {
            return options
                .IncludingAllRuntimeProperties()
                .Excluding(a => a.Id)
                .Excluding(a => a.Version)
                .Excluding(a => a.TimeStamp)
                .Excluding(a => a.EventStream);
        }
    }
}