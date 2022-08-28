using System;

namespace FutureNHS.WOPIHost.Exceptions
{
    public sealed class WopiDiscoveryDocumentEmptyException : ApplicationException
    {
        public WopiDiscoveryDocumentEmptyException() : base("The WOPI Discovery Document is Empty and cannot be used to perform the requested action") { }
    }
}
