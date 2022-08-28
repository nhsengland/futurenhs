using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    /// <summary>
    /// Singleton middleware handler for WOPI related requests that terminates the pipeline if we have a WOPI request to respond to
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware"/>
    public class WopiMiddleware 
    {
        private readonly RequestDelegate _next;

        public WopiMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));

            var isWopiRequest = await ProcessRequest(httpContext);

            if (isWopiRequest) return;

            if (httpContext.Response.HasStarted) return;
            if (httpContext.Response.StatusCode != StatusCodes.Status200OK) return;

            if (_next is null) return;

            await _next.Invoke(httpContext);
        }

        /// <summary>
        /// Tasked with identifying if the incoming request pertains to WOPI.  If it does, the relevant handler will be invoked
        /// to deal with it, otherwise we will just transparently ignore it
        /// </summary>
        /// <param name="httpContext">The context associated with the request</param>
        /// <returns>true if it was identified and handled, else false</returns>
        private static async Task<bool> ProcessRequest(HttpContext httpContext)
        {
            const bool THIS_IS_A_VALID_WOPI_REQUEST = true;
            const bool THIS_IS_NOT_A_VALID_WOPI_REQUEST = false;

            var cancellationToken = httpContext.RequestAborted;

            cancellationToken.ThrowIfCancellationRequested();

            var logger = httpContext.RequestServices.GetService<ILogger<WopiMiddleware>>();

            var wopiRequestFactory = httpContext.RequestServices.GetRequiredService<IWopiRequestHandlerFactory>();

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            if (wopiRequestHandler.IsEmpty) return THIS_IS_NOT_A_VALID_WOPI_REQUEST;

            Debug.Assert(wopiRequestHandler.DemandsProof.HasValue);

            logger?.LogTrace($"Looks like a WOPI request so going to try and route it to the correct handler");

            // TODO - Refactor for SRP and push to a delegate to verify the proof

            if (wopiRequestHandler.DemandsProof.Value)
            {
                logger?.LogTrace("Proof is demanded by the request handler - delegating to cryptographically verify the offered proof token: '{WOPIRequestHandlerType}'", wopiRequestHandler.GetType().Name);

                var wopiDiscoveryDocumentFactory = httpContext.RequestServices.GetRequiredService<IWopiDiscoveryDocumentFactory>();

                var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

                if (wopiDiscoveryDocument.IsEmpty) throw new ApplicationException("This is a WOPI request but the WOPI discovery document is temporarily unavilable/inaccessible and so the request cannot be processed");

                logger?.LogTrace($"Resolved discovery document to use so time to try and validate the proof offered by the caller");

                var wopiCryptoProofChecker = httpContext.RequestServices.GetRequiredService<IWopiCryptoProofChecker>();

                var (isInvalid, refetchProofKeys) = wopiCryptoProofChecker.IsProofInvalid(httpContext.Request, wopiDiscoveryDocument);

                if (refetchProofKeys || isInvalid) wopiDiscoveryDocument.IsTainted = true;

                if (isInvalid) throw new ApplicationException("This HttpRequest has been identified as a WOPI request but the proof(s) that have been presented by the caller cannot be verified to have been signed by a wopi-client trusted by the application.  If the request is from a valid source, it may be that it's signing keys have rotated without us knowing");

                logger?.LogTrace("Presented proof has been determined valid so routing to the handler of the request: '{WopiRequestHandlerTypeName}'", wopiRequestHandler.GetType().Name);
            }

            await wopiRequestHandler.HandleAsync(httpContext, cancellationToken);

            return THIS_IS_A_VALID_WOPI_REQUEST;
        }

        //private sealed class WopiHeaders
        //{
        //    public const string RequestType = "X-WOPI-Override";
        //    public const string ItemVersion = "X-WOPI-ItemVersion";

        //    public const string Lock = "X-WOPI-Lock";
        //    public const string OldLock = "X-WOPI-OldLock";
        //    public const string LockFailureReason = "X-WOPI-LockFailureReason";
        //    public const string LockedByOtherInterface = "X-WOPI-LockedByOtherInterface";

        //    public const string SuggestedTarget = "X-WOPI-SuggestedTarget";
        //    public const string RelativeTarget = "X-WOPI-RelativeTarget";
        //    public const string OverwriteRelativeTarget = "X-WOPI-OverwriteRelativeTarget";
        //}



        //private const string WopiPath = @"/wopi/";
        //private const string FilesRequestPath = @"files/";
        //private const string FoldersRequestPath = @"folders/";
        //private const string ContentsRequestPath = @"/contents";
        //private const string ChildrenRequestPath = @"/children";

        //private class LockInfo
        //{
        //    public string Lock { get; set; }
        //    public DateTime DateCreated { get; set; }
        //    public bool Expired { get { return this.DateCreated.AddMinutes(30) < DateTime.UtcNow; } }
        //}

        ///// <summary>
        ///// Simplified Lock info storage.
        ///// A real lock implementation would use persised storage for locks.
        ///// </summary>
        //private static readonly Dictionary<string, LockInfo> Locks = new Dictionary<string, LockInfo>();



        ///// <summary>
        ///// Parse the request determine the request type, access token, and file id.
        ///// For more details, see the [MS-WOPI] Web Application Open Platform Interface Protocol specification.
        ///// </summary>
        ///// <remarks>
        ///// Can be extended to parse client version, machine name, etc.
        ///// </remarks>
        //private static WopiRequest ParseRequest(HttpRequest request)
        //{
        //    // Initilize wopi request data object with default values
        //    WopiRequest requestData = new WopiRequest()
        //    {
        //        Type = RequestType.None,
        //        AccessToken = request.QueryString["access_token"],
        //        Id = ""
        //    };

        //    // request.Url pattern:
        //    // http(s)://server/<...>/wopi/[files|folders]/<id>?access_token=<token>
        //    // or
        //    // https(s)://server/<...>/wopi/files/<id>/contents?access_token=<token>
        //    // or
        //    // https(s)://server/<...>/wopi/folders/<id>/children?access_token=<token>

        //    // Get request path, e.g. /<...>/wopi/files/<id>
        //    string requestPath = request.Url.AbsolutePath;
        //    // remove /<...>/wopi/
        //    string wopiPath = requestPath.Substring(WopiPath.Length);

        //    if (wopiPath.StartsWith(FilesRequestPath))
        //    {
        //        // A file-related request

        //        // remove /files/ from the beginning of wopiPath
        //        string rawId = wopiPath.Substring(FilesRequestPath.Length);

        //        if (rawId.EndsWith(ContentsRequestPath))
        //        {
        //            // The rawId ends with /contents so this is a request to read/write the file contents

        //            // Remove /contents from the end of rawId to get the actual file id
        //            requestData.Id = rawId.Substring(0, rawId.Length - ContentsRequestPath.Length);

        //            if (request.HttpMethod == "GET")
        //                requestData.Type = RequestType.GetFile;
        //            if (request.HttpMethod == "POST")
        //                requestData.Type = RequestType.PutFile;
        //        }
        //        else
        //        {
        //            requestData.Id = rawId;

        //            if (request.HttpMethod == "GET")
        //            {
        //                // a GET to the file is always a CheckFileInfo request
        //                requestData.Type = RequestType.CheckFileInfo;
        //            }
        //            else if (request.HttpMethod == "POST")
        //            {
        //                // For a POST to the file we need to use the X-WOPI-Override header to determine the request type
        //                string wopiOverride = request.Headers[WopiHeaders.RequestType];

        //                switch (wopiOverride)
        //                {
        //                    case "PUT_RELATIVE":
        //                        requestData.Type = RequestType.PutRelativeFile;
        //                        break;
        //                    case "LOCK":
        //                        // A lock could be either a lock or an unlock and relock, determined based on whether
        //                        // the request sends an OldLock header.
        //                        if (request.Headers[WopiHeaders.OldLock] != null)
        //                            requestData.Type = RequestType.UnlockAndRelock;
        //                        else
        //                            requestData.Type = RequestType.Lock;
        //                        break;
        //                    case "UNLOCK":
        //                        requestData.Type = RequestType.Unlock;
        //                        break;
        //                    case "REFRESH_LOCK":
        //                        requestData.Type = RequestType.RefreshLock;
        //                        break;
        //                    case "COBALT":
        //                        requestData.Type = RequestType.ExecuteCobaltRequest;
        //                        break;
        //                    case "DELETE":
        //                        requestData.Type = RequestType.DeleteFile;
        //                        break;
        //                    case "READ_SECURE_STORE":
        //                        requestData.Type = RequestType.ReadSecureStore;
        //                        break;
        //                    case "GET_RESTRICTED_LINK":
        //                        requestData.Type = RequestType.GetRestrictedLink;
        //                        break;
        //                    case "REVOKE_RESTRICTED_LINK":
        //                        requestData.Type = RequestType.RevokeRestrictedLink;
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    else if (wopiPath.StartsWith(FoldersRequestPath))
        //    {
        //        // A folder-related request.

        //        // remove /folders/ from the beginning of wopiPath
        //        string rawId = wopiPath.Substring(FoldersRequestPath.Length);

        //        if (rawId.EndsWith(ChildrenRequestPath))
        //        {
        //            // rawId ends with /children, so it's an EnumerateChildren request.

        //            // remove /children from the end of rawId
        //            requestData.Id = rawId.Substring(0, rawId.Length - ChildrenRequestPath.Length);
        //            requestData.Type = RequestType.EnumerateChildren;
        //        }
        //        else
        //        {
        //            // rawId doesn't end with /children, so it's a CheckFolderInfo.

        //            requestData.Id = rawId;
        //            requestData.Type = RequestType.CheckFolderInfo;
        //        }
        //    }
        //    else
        //    {
        //        // An unknown request.
        //        requestData.Type = RequestType.None;
        //    }
        //    return requestData;
        //}

        //#region Processing for each of the WOPI operations

        ///// <summary>
        ///// Processes a CheckFileInfo request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on CheckFileInfo, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/CheckFileInfo.html
        ///// </remarks>
        //private void HandleCheckFileInfoRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: false))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    try
        //    {
        //        FileInfo fileInfo = new FileInfo(requestData.FullPath);

        //        if (!fileInfo.Exists)
        //        {
        //            ReturnFileUnknown(context.Response);
        //            return;
        //        }

        //        // For more info on CheckFileInfoResponse fields, see
        //        // https://wopi.readthedocs.io/projects/wopirest/en/latest/files/CheckFileInfo.html#response
        //        CheckFileInfoResponse responseData = new CheckFileInfoResponse()
        //        {
        //            // required CheckFileInfo properties
        //            BaseFileName = Path.GetFileName(requestData.FullPath),
        //            OwnerId = "documentOwnerId",
        //            Size = (int)fileInfo.Length,
        //            UserId = "user@contoso.com",
        //            Version = fileInfo.LastWriteTimeUtc.ToString("O" /* ISO 8601 DateTime format string */), // Using the file write time is an arbitrary choice.

        //            // optional CheckFileInfo properties
        //            BreadcrumbBrandName = "LocalStorage WOPI Host",
        //            BreadcrumbFolderName = fileInfo.Directory != null ? fileInfo.Directory.Name : "",
        //            BreadcrumbDocName = Path.GetFileNameWithoutExtension(requestData.FullPath),
        //            BreadcrumbBrandUrl = "http://" + context.Request.Url.Host,
        //            BreadcrumbFolderUrl = "http://" + context.Request.Url.Host,

        //            UserFriendlyName = "A WOPI User",

        //            SupportsLocks = true,
        //            SupportsUpdate = true,
        //            UserCanNotWriteRelative = true, /* Because this host does not support PutRelativeFile */

        //            ReadOnly = fileInfo.IsReadOnly,
        //            UserCanWrite = !fileInfo.IsReadOnly,
        //        };

        //        string jsonString = JsonConvert.SerializeObject(responseData);

        //        context.Response.Write(jsonString);
        //        ReturnSuccess(context.Response);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        ReturnFileUnknown(context.Response);
        //    }
        //}

        ///// <summary>
        ///// Processes a GetFile request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on GetFile, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/GetFile.html
        ///// </remarks>
        //private void HandleGetFileRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: false))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    try
        //    {
        //        // transmit file from local storage to the response stream.
        //        context.Response.TransmitFile(requestData.FullPath);
        //        context.Response.AddHeader(WopiHeaders.ItemVersion, GetFileVersion(requestData.FullPath));
        //        ReturnSuccess(context.Response);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        ReturnFileUnknown(context.Response);
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        ReturnFileUnknown(context.Response);
        //    }
        //}

        //private static string GetFileVersion(string filename)
        //{
        //    FileInfo fileInfo = new FileInfo(filename);
        //    return fileInfo.LastWriteTimeUtc.ToString("O" /* ISO 8601 DateTime format string */); // Using the file write time is an arbitrary choice.
        //}

        ///// <summary>
        ///// Processes a PutFile request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on PutFile, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/PutFile.html
        ///// </remarks>
        //private void HandlePutFileRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: true))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    string newLock = context.Request.Headers[WopiHeaders.Lock];
        //    LockInfo existingLock;
        //    bool hasExistingLock;

        //    lock (Locks)
        //    {
        //        hasExistingLock = TryGetLock(requestData.Id, out existingLock);
        //    }

        //    if (hasExistingLock && existingLock.Lock != newLock)
        //    {
        //        // lock mismatch/locked by another interface
        //        ReturnLockMismatch(context.Response, existingLock.Lock);
        //        return;
        //    }

        //    FileInfo putTargetFileInfo = new FileInfo(requestData.FullPath);

        //    // The WOPI spec allows for a PutFile to succeed on a non-locked file if the file is currently zero bytes in length.
        //    // This allows for a more efficient Create New File flow that saves the Lock roundtrips.
        //    if (!hasExistingLock && putTargetFileInfo.Length != 0)
        //    {
        //        // With no lock and a non-zero file, a PutFile could potentially result in data loss by clobbering
        //        // existing content.  Therefore, return a lock mismatch error.
        //        ReturnLockMismatch(context.Response, reason: "PutFile on unlocked file with current size != 0");
        //    }

        //    // Either the file has a valid lock that matches the lock in the request, or the file is unlocked
        //    // and is zero bytes.  Either way, proceed with the PutFile.
        //    try
        //    {
        //        // TODO: Should be replaced with proper file save logic to a real storage system and ensures write atomicity
        //        using (var fileStream = File.Open(requestData.FullPath, FileMode.Truncate, FileAccess.Write, FileShare.None))
        //        {
        //            context.Request.InputStream.CopyTo(fileStream);
        //        }
        //        context.Response.AddHeader(WopiHeaders.ItemVersion, GetFileVersion(requestData.FullPath));
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        ReturnFileUnknown(context.Response);
        //    }
        //    catch (IOException)
        //    {
        //        ReturnServerError(context.Response);
        //    }
        //}

        ///// <summary>
        ///// Processes a Lock request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on Lock, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/Lock.html
        ///// </remarks>
        //private void HandleLockRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: true))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    string newLock = context.Request.Headers[WopiHeaders.Lock];

        //    lock (Locks)
        //    {
        //        LockInfo existingLock;
        //        bool fLocked = TryGetLock(requestData.Id, out existingLock);
        //        if (fLocked && existingLock.Lock != newLock)
        //        {
        //            // There is a valid existing lock on the file and it doesn't match the requested lockstring.

        //            // This is a fairly common case and shouldn't be tracked as an error.  Office can store
        //            // information about a current session in the lock value and expects to conflict when there's
        //            // an existing session to join.
        //            ReturnLockMismatch(context.Response, existingLock.Lock);
        //        }
        //        else
        //        {
        //            // The file is not currently locked or the lock has already expired

        //            if (fLocked)
        //                Locks.Remove(requestData.Id);

        //            // Create and store new lock information
        //            // TODO: In a real implementation the lock should be stored in a persisted and shared system.
        //            Locks[requestData.Id] = new LockInfo() { DateCreated = DateTime.UtcNow, Lock = newLock };

        //            context.Response.AddHeader(WopiHeaders.ItemVersion, GetFileVersion(requestData.FullPath));

        //            // Return success
        //            ReturnSuccess(context.Response);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Processes a RefreshLock request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on RefreshLock, see
        ///// ttps://wopi.readthedocs.io/projects/wopirest/en/latest/files/RefreshLock.html
        ///// </remarks>
        //private void HandleRefreshLockRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: true))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    string newLock = context.Request.Headers[WopiHeaders.Lock];

        //    lock (Locks)
        //    {
        //        LockInfo existingLock;
        //        if (TryGetLock(requestData.Id, out existingLock))
        //        {
        //            if (existingLock.Lock == newLock)
        //            {
        //                // There is a valid lock on the file and the existing lock matches the provided one

        //                // Extend the lock timeout
        //                existingLock.DateCreated = DateTime.UtcNow;
        //                ReturnSuccess(context.Response);
        //            }
        //            else
        //            {
        //                // The existing lock doesn't match the requested one.  Return a lock mismatch error
        //                // along with the current lock
        //                ReturnLockMismatch(context.Response, existingLock.Lock);
        //            }
        //        }
        //        else
        //        {
        //            // The requested lock does not exist.  That's also a lock mismatch error.
        //            ReturnLockMismatch(context.Response, reason: "File not locked");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Processes a Unlock request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on Unlock, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/Unlock.html
        ///// </remarks>
        //private void HandleUnlockRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: true))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    string newLock = context.Request.Headers[WopiHeaders.Lock];

        //    lock (Locks)
        //    {
        //        LockInfo existingLock;
        //        if (TryGetLock(requestData.Id, out existingLock))
        //        {
        //            if (existingLock.Lock == newLock)
        //            {
        //                // There is a valid lock on the file and the existing lock matches the provided one

        //                // Remove the current lock
        //                Locks.Remove(requestData.Id);
        //                context.Response.AddHeader(WopiHeaders.ItemVersion, GetFileVersion(requestData.FullPath));
        //                ReturnSuccess(context.Response);
        //            }
        //            else
        //            {
        //                // The existing lock doesn't match the requested one.  Return a lock mismatch error
        //                // along with the current lock
        //                ReturnLockMismatch(context.Response, existingLock.Lock);
        //            }
        //        }
        //        else
        //        {
        //            // The requested lock does not exist.  That's also a lock mismatch error.
        //            ReturnLockMismatch(context.Response, reason: "File not locked");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Processes a UnlockAndRelock request
        ///// </summary>
        ///// <remarks>
        ///// For full documentation on UnlockAndRelock, see
        ///// https://wopi.readthedocs.io/projects/wopirest/en/latest/files/UnlockAndRelock.html
        ///// </remarks>
        //private void HandleUnlockAndRelockRequest(HttpContext context, WopiRequest requestData)
        //{
        //    if (!ValidateAccess(requestData, writeAccessRequired: true))
        //    {
        //        ReturnInvalidToken(context.Response);
        //        return;
        //    }

        //    if (!File.Exists(requestData.FullPath))
        //    {
        //        ReturnFileUnknown(context.Response);
        //        return;
        //    }

        //    string newLock = context.Request.Headers[WopiHeaders.Lock];
        //    string oldLock = context.Request.Headers[WopiHeaders.OldLock];

        //    lock (Locks)
        //    {
        //        LockInfo existingLock;
        //        if (TryGetLock(requestData.Id, out existingLock))
        //        {
        //            if (existingLock.Lock == oldLock)
        //            {
        //                // There is a valid lock on the file and the existing lock matches the provided one

        //                // Replace the existing lock with the new one
        //                Locks[requestData.Id] = new LockInfo() { DateCreated = DateTime.UtcNow, Lock = newLock };
        //                context.Response.Headers[WopiHeaders.OldLock] = newLock;
        //                ReturnSuccess(context.Response);
        //            }
        //            else
        //            {
        //                // The existing lock doesn't match the requested one.  Return a lock mismatch error
        //                // along with the current lock
        //                ReturnLockMismatch(context.Response, existingLock.Lock);
        //            }
        //        }
        //        else
        //        {
        //            // The requested lock does not exist.  That's also a lock mismatch error.
        //            ReturnLockMismatch(context.Response, reason: "File not locked");
        //        }
        //    }
        //}

        //#endregion



        ///// <summary>
        ///// Validate that the provided access token is valid to get access to requested resource.
        ///// </summary>
        ///// <param name="requestData">Request information, including requested file Id</param>
        ///// <param name="writeAccessRequired">Whether write permission is requested or not.</param>
        ///// <returns>true when access token is correct and user has access to document, false otherwise.</returns>
        //private static bool ValidateAccess(WopiRequest requestData, bool writeAccessRequired)
        //{
        //    // TODO: Access token validation is not implemented in this sample.
        //    // For more details on access tokens, see the documentation
        //    // https://wopi.readthedocs.io/projects/wopirest/en/latest/concepts.html#term-access-token
        //    // "INVALID" is used by the WOPIValidator.
        //    return !String.IsNullOrWhiteSpace(requestData.AccessToken) && (requestData.AccessToken != "INVALID");
        //}

        //private static void ReturnSuccess(HttpResponse response)
        //{
        //    ReturnStatus(response, 200, "Success");
        //}

        //private static void ReturnInvalidToken(HttpResponse response)
        //{
        //    ReturnStatus(response, 401, "Invalid Token");
        //}

        //private static void ReturnFileUnknown(HttpResponse response)
        //{
        //    ReturnStatus(response, 404, "File Unknown/User Unauthorized");
        //}

        //private static void ReturnLockMismatch(HttpResponse response, string existingLock = null, string reason = null)
        //{
        //    response.Headers[WopiHeaders.Lock] = existingLock ?? String.Empty;
        //    if (!String.IsNullOrEmpty(reason))
        //    {
        //        response.Headers[WopiHeaders.LockFailureReason] = reason;
        //    }

        //    ReturnStatus(response, 409, "Lock mismatch/Locked by another interface");
        //}

        //private static void ReturnServerError(HttpResponse response)
        //{
        //    ReturnStatus(response, 500, "Server Error");
        //}

        //private static void ReturnUnsupported(HttpResponse response)
        //{
        //    ReturnStatus(response, 501, "Unsupported");
        //}

        //private static void ReturnStatus(HttpResponse response, int code, string description)
        //{
        //    response.StatusCode = code;
        //    response.StatusDescription = description;
        //}

        //private bool TryGetLock(string fileId, out LockInfo lockInfo)
        //{
        //    // TODO: This lock implementation is not thread safe and not persisted and all in all just an example.
        //    if (Locks.TryGetValue(fileId, out lockInfo))
        //    {
        //        if (lockInfo.Expired)
        //        {
        //            Locks.Remove(fileId);
        //            return false;
        //        }
        //        return true;
        //    }

        //    return false;
        //}
    }
}
