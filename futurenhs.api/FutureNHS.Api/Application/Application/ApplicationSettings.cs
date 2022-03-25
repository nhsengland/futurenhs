using FutureNHS.Application.Interfaces;

namespace FutureNHS.Application.Application
{
    public sealed class ApplicationSettings : IApplicationSettings
    {
        public string ReadOnlyDbConnectionString { get; }
        public string WriteOnlyDbConnectionString { get; }
        public int RetryAttempts { get; }
        public int RetryDelay { get; }
        public int MaxPageSize { get; }
        public ApplicationSettings(string readOnlyDbConnectionString, string writeOnlyDbConnectionString, int retryAttempts, int retryDelay)
        {
            if (string.IsNullOrWhiteSpace(readOnlyDbConnectionString))
            {
                throw new ArgumentNullException(nameof(readOnlyDbConnectionString));
            }

            if (string.IsNullOrWhiteSpace(writeOnlyDbConnectionString))
            {
                throw new ArgumentNullException(nameof(writeOnlyDbConnectionString));
            }

            if (retryAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryAttempts));
            }

            if (retryDelay <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelay));
            }

            ReadOnlyDbConnectionString = readOnlyDbConnectionString;
            WriteOnlyDbConnectionString = writeOnlyDbConnectionString;
            RetryAttempts = retryAttempts;
            RetryDelay = retryDelay;
        }
    }
}