namespace FutureNHS.Api.Exceptions
{
    public sealed class PreconditionFailedExeption : ApplicationException
    {
        public PreconditionFailedExeption(string message) : base(message) { }
    }
}
