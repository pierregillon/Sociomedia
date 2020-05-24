using System;

namespace Sociomedia.Articles.Domain {
    public class UnreachableWebDocumentException : Exception
    {
        public UnreachableWebDocumentException() : base("Web document unreachable.") { }
        public UnreachableWebDocumentException(Exception innerException) : base("Web document unreachable.", innerException) { }
    }
}