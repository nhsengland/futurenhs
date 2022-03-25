namespace FutureNHS.Api.Exceptions
{
    public class NotModifiedException : ApplicationException
    {
        public NotModifiedException(string message) : base(message) { }
    }
}
