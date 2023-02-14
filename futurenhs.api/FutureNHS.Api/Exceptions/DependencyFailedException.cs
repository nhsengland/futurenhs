namespace FutureNHS.Api.Exceptions
{
    public sealed class DependencyFailedException : ApplicationException
    {
        public DependencyFailedException(string message) : base(message) { }

    }
}
