using Microsoft.AspNetCore.Http;
using System;

namespace FutureNHS.WOPIHost
{
    public static partial class ExtensionMethods
    {
        internal const string WOPI_PATH_SEGMENT = "/wopi";
        internal const string WOPI_HEALTH_CHECK_PATH_SEGMENT = WOPI_PATH_SEGMENT + "/health-check";
        internal const string WOPI_COLLABORA_TEST_PAGE_PATH_SEGMENT = WOPI_PATH_SEGMENT + "/collabora";
        internal const string WOPI_FILES_PATH_SEGMENT = WOPI_PATH_SEGMENT + "/files";

        public static bool IsCollaboraTestPage(this PathString requestPath) => requestPath.StartsWithSegments(WOPI_COLLABORA_TEST_PAGE_PATH_SEGMENT, StringComparison.OrdinalIgnoreCase);
        public static bool IsHealthCheck(this PathString requestPath) => requestPath.StartsWithSegments(WOPI_HEALTH_CHECK_PATH_SEGMENT, StringComparison.OrdinalIgnoreCase);
        public static bool IsWopiPath(this PathString requestPath) => requestPath.StartsWithSegments(WOPI_PATH_SEGMENT, StringComparison.OrdinalIgnoreCase);
        public static bool IsWopiFilePath(this PathString requestPath) => requestPath.StartsWithSegments(WOPI_FILES_PATH_SEGMENT, StringComparison.OrdinalIgnoreCase);
    }
}
