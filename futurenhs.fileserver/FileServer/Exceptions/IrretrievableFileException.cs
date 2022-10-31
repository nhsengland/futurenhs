using System;

namespace FutureNHS.WOPIHost.Exceptions
{
    public class IrretrievableFileException : ApplicationException
    {
        public IrretrievableFileException(string? message) : base(message) { }
    }
}
