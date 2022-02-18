namespace FutureNHS.Application.Interfaces
{
    public interface IApplicationSettings
    {
        string WriteOnlyDbConnectionString { get; }
        string ReadOnlyDbConnectionString { get; }
        int RetryAttempts { get; }
        int RetryDelay { get; } 
        int MaxPageSize { get; }
    }
}
