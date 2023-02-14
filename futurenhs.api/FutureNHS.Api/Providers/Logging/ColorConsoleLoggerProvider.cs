namespace FutureNHS.Api.Providers.Logging
{
    using Microsoft.Extensions.Logging;
    using System.Collections.Concurrent;
    using System.Runtime.Versioning;

    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("ColorConsole")]
    public sealed class ColorConsoleLoggerProvider : ILoggerProvider
    {
 
        private ColorConsoleLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, ColorConsoleLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public ColorConsoleLoggerProvider()
        {
            _currentConfig = new ColorConsoleLoggerConfiguration();
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new ColorConsoleLogger(name, GetCurrentConfig));

        private ColorConsoleLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();

        }
    }
}
