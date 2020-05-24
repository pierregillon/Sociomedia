using System;

namespace Sociomedia.Core
{
    public static class TypeExtensions
    {
        public static string DisplayableName(this Type type)
        {
            return type.Name.SeparatePascalCaseWords().ToUpper();
        }
    }
}