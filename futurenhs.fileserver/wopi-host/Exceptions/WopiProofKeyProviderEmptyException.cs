using System;

namespace FutureNHS.WOPIHost.Exceptions
{
    public sealed class WopiProofKeyProviderEmptyException : ApplicationException
    {
        public WopiProofKeyProviderEmptyException() : base("The WOPI Proof Key Provider does not have any keys and cannot be used to perform the requested action") { }
    }
}
