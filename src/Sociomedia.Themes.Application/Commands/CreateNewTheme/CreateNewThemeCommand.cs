using System.Collections.Generic;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.CreateNewTheme
{
    public class CreateNewThemeCommand : ICommand
    {
        private readonly KeywordIntersection _intersection;
        public IReadOnlyCollection<Article> Articles { get; }

        public CreateNewThemeCommand(KeywordIntersection intersection, IReadOnlyCollection<Article> articles)
        {
            _intersection = intersection;
            Articles = articles;
        }

        protected bool Equals(CreateNewThemeCommand other)
        {
            return _intersection.Equals(other._intersection);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CreateNewThemeCommand) obj);
        }

        public override int GetHashCode()
        {
            return _intersection.GetHashCode();
        }
    }
}