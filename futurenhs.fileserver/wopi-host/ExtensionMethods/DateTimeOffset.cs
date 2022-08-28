using System;
using System.Globalization;

namespace FutureNHS.WOPIHost
{
    public static partial class ExtensionMethods
    {
        public static string? ToIso8601(this DateTimeOffset? dateTimeOffset) =>
            dateTimeOffset.HasValue 
            ? dateTimeOffset.Value.ToIso8601()
            : default;

        public static string ToIso8601(this DateTimeOffset dateTimeOffset) =>
            dateTimeOffset.ToString("o", CultureInfo.InvariantCulture);
    }
}
