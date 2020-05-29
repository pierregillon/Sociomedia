using System;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class ThemeEvent : DomainEvent
    {
        protected ThemeEvent(Guid id) : base(id, "theme") { }
    }
}