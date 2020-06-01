using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.CreateNewTheme
{
    public class CreateNewThemeCommand : ICommand
    {
        public IReadOnlyCollection<Keyword2> Keywords { get; }
        public IReadOnlyCollection<Guid> Articles { get; }

        public CreateNewThemeCommand(IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles)
        {
            Keywords = keywords;
            Articles = articles;
        }

        protected bool Equals(CreateNewThemeCommand other)
        {
            return Keywords.Select(x => x.Value).SequenceEqual(other.Keywords.Select(x => x.Value)) && Articles.SequenceEqual(other.Articles);
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
                foreach (var keyword in Keywords) {
                    hash = hash * 31 + keyword.Value.GetHashCode();
                }
                foreach (var article in Articles) {
                    hash = hash * 31 + article.GetHashCode();
                }
                return hash;
            }
        }
    }
}