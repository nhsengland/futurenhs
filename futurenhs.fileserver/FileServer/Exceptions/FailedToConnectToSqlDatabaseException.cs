namespace FileServer.Exceptions
{
    public sealed class FailedToConnectToSqlDatabaseException : ApplicationException
    {
        public FailedToConnectToSqlDatabaseException(string? message) : base(message) { }
    }
}
