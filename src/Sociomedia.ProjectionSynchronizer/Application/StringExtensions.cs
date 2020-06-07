using System.Linq;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public static class StringExtensions
    {
        public static string FirstLetterUpper(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }
            if (char.IsUpper(value[0])) {
                return value;
            }
            return new string(value.Skip(1).Prepend(char.ToUpper(value[0])).ToArray());
        }
    }
}