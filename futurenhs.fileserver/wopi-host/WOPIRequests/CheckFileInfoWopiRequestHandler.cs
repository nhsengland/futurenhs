﻿using FutureNHS.WOPIHost.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.WOPIRequests
{
    internal sealed class CheckFileInfoWopiRequestHandler
        : WopiRequestHandler
    {
        private readonly AuthenticatedUser _authenticatedUser;
        private readonly File _file;
        private readonly Features? _features;

        private CheckFileInfoWopiRequestHandler(AuthenticatedUser authenticatedUser, File file, Features? features) 
            : base(true) 
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));

            _file = file;

            _features = features;

            _authenticatedUser = authenticatedUser;
        }

        internal static CheckFileInfoWopiRequestHandler With(AuthenticatedUser authenticatedUser, File file, Features features) => new(authenticatedUser, file, features);

        protected override async Task<int> HandleAsyncImpl(HttpContext httpContext, CancellationToken cancellationToken)
        {
            // GET /wopi/files/(file_id) 
            //
            // https://wopi.readthedocs.io/projects/wopirest/en/latest/files/CheckFileInfo.html#checkfileinfo
            // https://sdk.collaboraonline.com/docs/advanced_integration.html
            // The CheckFileInfo operation is one of the most important WOPI operations. This operation must be implemented for all
            // WOPI actions. CheckFileInfo returns information about a file, a user’s permissions on that file, and general information
            // about the capabilities that the WOPI host has on the file. In addition, some CheckFileInfo properties can influence the 
            // appearance and behavior of WOPI clients.
            //
            // NB - Collabora does not implement the full WOPI specification and thus only meets the bare minimum specification.
            //      You can find more information in their 'Integration of Collabora Online with WOPI' document

            var fileMetadataProvider = httpContext.RequestServices.GetRequiredService<IUserFileMetadataProvider>();

            var fileMetadata = await fileMetadataProvider.GetForFileAsync(_file, _authenticatedUser, cancellationToken);
            
            if (fileMetadata is null) return StatusCodes.Status404NotFound;
            if (!fileMetadata.UserHasViewPermission) return StatusCodes.Status403Forbidden;

            // TODO - userMetadata = Get user context from the authenticated user
            // Note that there is no property that indicates the user has permission to read/view a file. This is because WOPI requires 
            // the host to respond to any WOPI request, including CheckFileInfo, with a 401 Unauthorized or 404 Not Found if the access 
            // token is invalid or expired.

            dynamic responseBody = new ExpandoObject();

            // Mandatory
            // Hosts use the OwnerId and UserId properties to provide user ID data to WOPI clients. User identity properties are intended for telemetry purposes, and thus should not be shown in any WOPI client UI

            responseBody.BaseFileName = fileMetadata.Title;                          // The string name of the file, including extension, without a path. Used for display in user interface (UI), and determining the extension of the file.
            responseBody.OwnerId = fileMetadata.OwnerUserName;                       // A string that uniquely identifies the owner of the file. In most cases, the user who uploaded or created the file should be considered the owner.
            responseBody.Size = fileMetadata.SizeInBytes;                            // The size of the file in bytes, expressed as a long, a 64-bit signed integer.
            responseBody.UserId = _authenticatedUser.Id.ToString();                  // A string value uniquely identifying the user currently accessing the file.
            responseBody.Version = fileMetadata.FileVersion;                         // The current version of the file based on the server’s file version schema, as a string. This value must change when the file changes, and version values must never repeat for a given file.

            // Host capabilities
            // The WOPI host capabilities properties indicate to the WOPI client what WOPI capabilities that the host supports for a 
            // file. All of these properties are optional and thus default to false; hosts should set them to true if their WOPI 
            // implementation meets the requirements for a particular property.

            var supportsUpdate = _features is not null && _features.AllowFileEdit; // && fileMetadata.HasEditPermission;

            responseBody.SupportedShareUrlType = new[] { "ReadOnly" };               // ReadOnly | ReadWrite - An array of strings containing the Share URL types supported by the host.  These types can be passed in the X-WOPI - UrlType request header to signify which Share URL type to return for the GetShareUrl (files) operation.
            responseBody.SupportsCobalt = false;                                     // A Boolean value that indicates that the host supports the ExecuteCellStorageRequest and ExecuteCellStorageRelativeRequest WOPI operations
            responseBody.SupportsContainers = false;                                 // A Boolean value that indicates that the host supports the CheckContainerInfo, CreateChildContainer, CreateChildFile, DeleteContainer, DeleteFile, EnumerateAncestors (containers), EnumerateAncestors (files), EnumerateChildren (containers), GetEcosystem (containers) and RenameContainer WOPI operations
            responseBody.SupportsDeleteFile = false;                                 // A Boolean value that indicates that the host supports the DeleteFile operation.
            responseBody.SupportsEcosystem = false;                                  // A Boolean value that indicates that the host supports the CheckEcosystem, GetEcosystem (containers), GetEcosystem (files), and GetRootContainer (ecosystem) WOPI operations
            responseBody.SupportsExtendedLockLength = true;                          // A Boolean value that indicates that the host supports lock IDs up to 1024 ASCII characters long. If not provided, WOPI clients will assume that lock IDs are limited to 256 ASCII characters.
            responseBody.SupportsFolders = false;                                    // A Boolean value that indicates that the host supports the CheckFolderInfo, EnumerateChildren (folders) and DeleteFile WOPI operations
            responseBody.SupportsGetFileWopiSrc = false;                             // A Boolean value that indicates that the host supports the 🚧 GetFileWopiSrc (ecosystem) operation.
            responseBody.SupportsGetLock = false;                                    // A Boolean value that indicates that the host supports the GetLock operation.
            responseBody.SupportsLocks = false;                                      // A Boolean value that indicates that the host supports the Lock, Unlock, RefreshLock and UnlockAndRelock WOPI operations
            responseBody.SupportsRename = false;                                     // A Boolean value that indicates that the host supports the RenameFile operation.
            responseBody.SupportsUpdate = supportsUpdate;                            // A Boolean value that indicates that the host supports the PutFile and PutRelativeFile WOPI operations
            responseBody.SupportsUserInfo = false;                                   // A Boolean value that indicates that the host supports the PutUserInfo operation.

            // User metadata

            responseBody.IsAnonymousUser = false;                                    // A Boolean value indicating whether the user is authenticated with the host or not. Hosts should always set this to true for unauthenticated users, so that clients are aware that the user is anonymous.
            responseBody.IsEduUser = false;                                          // A Boolean value indicating whether the user is an education user or not.
            responseBody.LicenseCheckForEditIsEnabled = false;                       // A Boolean value indicating whether the user is a business user or not.
            responseBody.UserFriendlyName = _authenticatedUser.FullName;             // A string that is the name of the user, suitable for displaying in UI. Eg when making comments in a file (can we inject avatar some how)?

            // User permissions

            responseBody.ReadOnly = !supportsUpdate;                                 // A Boolean value that indicates that, for this user, the file cannot be changed.
            responseBody.RestrictedWebViewOnly = false;                              // A Boolean value that indicates that the WOPI client should restrict what actions the user can perform on the file. The behavior of this property is dependent on the WOPI client.
            responseBody.UserCanAttend = false;                                      // A Boolean value that indicates that the user has permission to view a broadcast of this file
            responseBody.UserCanNotWriteRelative = true;                             // A Boolean value that indicates the user does not have sufficient permission to create new files on the WOPI server. Setting this to true tells the WOPI client that calls to PutRelativeFile will fail for this user on the current file.
            responseBody.UserCanPresent = false;                                     // A Boolean value that indicates that the user has permission to broadcast this file to a set of users who have permission to broadcast or view a broadcast of the current file.
            responseBody.UserCanRename = false;                                      // A Boolean value that indicates the user has permission to rename the current file.
            responseBody.UserCanWrite = supportsUpdate;                              // A Boolean value that indicates that the user has permission to alter the file. Setting this to true tells the WOPI client that it can call PutFile on behalf of the user.

            dynamic userExtraInfo = new ExpandoObject();

            var userAvatarUri = _authenticatedUser.UserAvatar?.Source;
            var userEmailAddress = _authenticatedUser.EmailAddress;

            if (userAvatarUri is not null && userAvatarUri.IsAbsoluteUri) userExtraInfo.Avatar = userAvatarUri.AbsoluteUri;
            if (!string.IsNullOrWhiteSpace(userEmailAddress)) userExtraInfo.Mail = userEmailAddress;

            responseBody.UserExtraInfo = userExtraInfo;

            // File URLs

            // TODO - link to where user can see file version history once we know what that is
            //var fileVersionUrl = string.Empty;

            // TODO - link to the sign out page for the app once it is known
            //var signOutUrl = string.Empty;

            //responseBody.CloseUrl = "";                                            // A URI to a web page that the WOPI client should navigate to when the application closes, or in the event of an unrecoverable error.
            //responseBody.DownloadUrl = ""; // downloadUrl;                                  // This URI should always provide the most recent version of the file - A user-accessible URI to the file intended to allow the user to download a copy of the file
            //responseBody.FileEmbedCommandUrl = "";                                 // A URI to a location that allows the user to create an embeddable URI to the file.
            //responseBody.FileSharingUrl = "";                                      // A URI to a location that allows the user to share the file.
            //responseBody.FileUrl = default(string);                                  // A URI to the file location that the WOPI client uses to get the file. If this is provided, the WOPI client may use this URI to get the file instead of a GetFile request. A host might set this property if it is easier or provides better performance to serve files from a different domain than the one handling standard WOPI requests. WOPI clients must not add or remove parameters from the URL; no other parameters, including the access token, should be appended to the FileUrl before it is used.
            //responseBody.FileVersionUrl = fileVersionUrl;                            // A URI to a location that allows the user to view the version history for the file.
            //responseBody.HostEmbeddedViewUrl = "";                                 // A URI to a web page that provides access to a viewing experience for the file that can be embedded in another HTML page. This is typically a URI to a host page that loads the embedview WOPI action.
            //responseBody.HostEditUrl = "";                                         // A URI to a host page that loads the edit WOPI action.
            //responseBody.HostViewUrl = "";                                         // A URI to a host page that loads the view WOPI action. This URL is used by Office Online to navigate between view and edit mode.
            //responseBody.SignOutUrl = signOutUrl;                                    // A URI that will sign the current user out of the host’s authentication system.

            // PostMessage properties for web-based WOPI clients
            // CheckFileInfo supports a number of properties that can be used by web-based WOPI clients such as Office for the web to 
            // customize the user interface and experience when using those clients. See PostMessage properties for more information on
            // these properties and how to use them

            // Breadcrumb properties
            // Breadcrumb properties are used by some WOPI clients to display breadcrumb-style navigation elements within the WOPI client UI.

            // TODO - Set BreadcrumbFolderUrl to url for the group containing the file?

            responseBody.BreadcrumbBrandName = "FutureNHS Open";                     // A string that indicates the brand name of the host
            responseBody.BreadcrumbBrandUrl = string.Empty;                          // A URI to a web page that the WOPI client should navigate to when the user clicks on UI that displays BreadcrumbBrandName.
            responseBody.BreadcrumbDocName = fileMetadata.Title;                     // A string that indicates the name of the file. If this is not provided, WOPI clients may use the BaseFileName value.
            responseBody.BreadcrumbFolderName = fileMetadata.GroupName;              // A string that indicates the name of the container that contains the file.
            responseBody.BreadcrumbFolderUrl = string.Empty;                         // A URI to a web page that the WOPI client should navigate to when the user clicks on UI that displays BreadcrumbFolderName.

            // Miscellaneous
            // TODO - Investigate the use of ClientThrottlingProtection and RequestedCallThrottling

            responseBody.AllowAdditionalMicrosoftServices = false;                   // A Boolean value that indicates a WOPI client may connect to Microsoft services to provide end-user functionality. Eg Bing Spelling
            responseBody.AllowErrorReportPrompt = false;                             // A Boolean value that indicates that in the event of an error, the WOPI client is permitted to prompt the user for permission to collect a detailed report about their specific error. The information gathered could include the user’s file and other session-specific state.
            responseBody.AllowExternalMarketplace = false;                           // A Boolean value that indicates a WOPI client may allow connections to external services referenced in the file (for example, a marketplace of embeddable JavaScript apps).
            responseBody.ClientThrottlingProtection = "Normal";                      // MostProtected | Protected | Normal | LessProtected | LeastProtected - A string value offering guidance to the WOPI client as to how to differentiate client throttling behaviors between the user and documents combinations from the WOPI host. Under times of stress, the WOPI client may choose to make use of this field to vary the level of impact of client side throttling behaviors within the set of active host documents. If the WOPI client chooses to differentiate throttling of client behaviors that are not necessarily tied to WOPI calls to the host, it may apply the most reduced quality of service to the LeastProtected document/users and the least reduced quality of service to the MostProtected documents/users. As in the case of RequestedCallThrottling, it is advised that hosts sharing this value between responses for distinct users of the same document at any given time may yield more deterministic results from the clients.
            responseBody.CloseButtonClosesWindow = false;                            // A Boolean value that indicates the WOPI client should close the window or tab when the user activates any Close UI in the WOPI client.
            responseBody.CopyPasteRestrictions = "CurrentDocumentOnly";              // BlockAll | CurrentDocumentOnly - A string value indicating whether the WOPI client should disable Copy and Paste functionality
            responseBody.DisablePrint = false;                                       // A Boolean value that indicates the WOPI client should disable all print functionality.
            responseBody.DisableTranslation = false;                                 // A Boolean value that indicates the WOPI client should disable all machine translation functionality.
            responseBody.FileExtension = fileMetadata.Extension;                     // A string value representing the file extension for the file. This value must begin with a period. If provided, WOPI clients will use this value as the file extension. Otherwise the extension will be parsed from the BaseFileName.
            responseBody.FileNameMaxLength = File.FILENAME_MAXIMUM_LENGTH;           // An integer value that indicates the maximum length for file names that the WOPI host supports, excluding the file extension. The default value is 250. Note that WOPI clients will use this default value if the property is omitted or if it is explicitly set to 0
            responseBody.LastModifiedTime = fileMetadata.LastWriteTimeUtc.ToIso8601();  // A string that represents the last time that the file was modified. This time must always be a must be a UTC time, and must be formatted in ISO 8601 round-trip format. For example, "2009-06-15T13:45:30.0000000Z".
            responseBody.RequestedCallThrottling = "Normal";                         // Normal | Minor | Medium | Major | Critical - A string value indicating whether the WOPI host is experiencing capacity problems and would like to reduce the frequency at which the WOPI clients make calls to the host. Each WOPI application may choose how best to respect the expressed desire from the host. WOPI applications may respond in manners such as reducing the frequency of CheckFileInfo calls and extending the window between when a user makes a change and the updated document gets saved back to the WOPI host. It is advised that hosts sharing this value between responses for distinct users of the same document at any given time may yield more deterministic results from the clients.
            responseBody.SHA256 = fileMetadata.ContentHash;                          // A 256 bit SHA-2-encoded [FIPS 180-2] hash of the file contents, as a Base64-encoded string. Used for caching purposes in WOPI clients.
            responseBody.SharingStatus = "Private";                                  // Private | Shared - A string value indicating whether the current document is shared with other users. The value can change upon adding or removing permissions to other users. Clients should use this value to help decide when to enable collaboration features as a document must be Shared in order to multi-user collaboration on the document.
            responseBody.TemporarilyNotWritable = false;                             // A Boolean value that indicates that if host is temporarily unable to process writes on a file
            responseBody.UniqueContentId = _file.Id;                                 // In special cases, a host may choose to not provide a SHA256, but still have some mechanism for identifying that two different files contain the same content in the same manner as the SHA256 is used. This string value can be provided rather than a SHA256 value if and only if the host can guarantee that two different files with the same content will have the same UniqueContentId value.

            // Collabora specific optional properties - do we need to make this configurable and thus use feature flags for them?

            //responseBody.PostMessageOrigin = ;
            responseBody.HidePrintOption = false;
            responseBody.DisablePrint = false;
            responseBody.HideSaveOption = false;
            responseBody.HideExportOption = false;
            responseBody.DisableExport = false;
            responseBody.DisableCopy = false;
            responseBody.EnableOwnerTermination = false;

            await httpContext.Response.WriteAsJsonAsync((ExpandoObject)responseBody, cancellationToken: cancellationToken);

            return StatusCodes.Status200OK;
        }
    }
}
