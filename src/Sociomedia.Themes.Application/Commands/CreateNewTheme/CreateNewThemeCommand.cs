using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.CreateNewTheme
{
    public class CreateNewThemeCommand : ICommand
    {
        public IReadOnlyCollection<Article> Articles { get; }

        public CreateNewThemeCommand(IReadOnlyCollection<Article> articles)
        {
            Articles = articles;
        }

        protected bool Equals(CreateNewThemeCommand other)
        {
            return Articles.Select(x => x.Id).SequenceEqual(other.Articles.Select(x => x.Id));
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
            unchecked {
                var hash = 19;
                foreach (var article in Articles) {
                    hash = hash * 31 + article.GetHashCode();
                }
                return hash;
            }
        }
    }
}