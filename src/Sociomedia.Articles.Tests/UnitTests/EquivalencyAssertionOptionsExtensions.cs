using FluentAssertions.Equivalency;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Tests.UnitTests
{
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