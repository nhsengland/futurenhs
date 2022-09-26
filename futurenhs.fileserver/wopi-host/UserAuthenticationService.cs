using FutureNHS.WOPIHost.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IUserAuthenticationService
    {
        Task<AuthenticatedUser?> GetForFileContextAsync(HttpContext httpContext, File file, CancellationToken cancellationToken);

        Task<UserFileAccessToken> GenerateAccessToken(AuthenticatedUser authenticatedUser, File file, FileAccessPermission fileAccessPermission, CancellationToken cancellationToken);    
    }
        
    public sealed class UserAuthenticationService
        : IUserAuthenticationService
    {
        private readonly ILogger<UserAuthenticationService>? _logger;
        private readonly ISystemClock _systemClock;
        private readonly AppConfiguration _appConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserFileMetadataProvider _userFileMetadataProvider;
        private readonly IUserFileAccessTokenRepository _userFileAccessTokenRepository;

        public UserAuthenticationService(IUserFileMetadataProvider userFileMetadataProvider, IUserFileAccessTokenRepository userFileAccessTokenRepository, IHttpClientFactory httpClientFactory, IOptionsSnapshot<AppConfiguration> appConfiguration, ISystemClock systemClock, ILogger<UserAuthenticationService>? logger)
        {
            _logger = logger;

            _systemClock = systemClock                                     ?? throw new ArgumentNullException(nameof(systemClock));

            _userFileAccessTokenRepository = userFileAccessTokenRepository ?? throw new ArgumentNullException(nameof(userFileAccessTokenRepository));
            _userFileMetadataProvider = userFileMetadataProvider           ?? throw new ArgumentNullException(nameof(userFileMetadataProvider));
            _httpClientFactory = httpClientFactory                         ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _appConfiguration = appConfiguration?.Value                    ?? throw new ArgumentNullException(nameof(appConfiguration));

            var mvcForumUserInfoUrl = _appConfiguration.MvcForumUserInfoUrl;

            if (mvcForumUserInfoUrl is null) throw new ApplicationException($"The {nameof(AppConfiguration.MvcForumUserInfoUrl)} is null");

            if (!mvcForumUserInfoUrl.IsAbsoluteUri) throw new ApplicationException($"The {nameof(AppConfiguration.MvcForumUserInfoUrl)} is not an absolute URI = {mvcForumUserInfoUrl}");
        }

        async Task<AuthenticatedUser?> IUserAuthenticationService.GetForFileContextAsync(HttpContext httpContext, File file, CancellationToken cancellationToken)
        {
            if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));

            cancellationToken.ThrowIfCancellationRequested();

            // There are three mechanisms in use to authenticate a request:
            //
            // 1. An ASP Authentication Cookie produced by the Forum application may be presented.  If present, we need to call across to 
            //    the forum application to verify and renew it.
            // 2. An access_token query parameter is used when a cookie cannot be.  For example, communication between the browser and the 
            //    Wopi Client may be over a web socket.  While passing a cookie is technically possible, it would open to door to x-site attacks
            //    given WSS doesn't enforce the same security policies as HTTPS
            // 3. Where a proof header is sent to verify the source of the request is trusted (a Wopi Client we trust), the signature includes the
            //    access token that is also provided as a query parameter.  We can verify the access token independently and tie it back to a 
            //    specific user and valid window
            //
            // This service is responsible for checking the first two methods

            var httpRequest = httpContext.Request;

            var authenticatedUserByCookie = await AuthenticateCookieAsync(httpRequest, cancellationToken);

            var authenticatedUserByAccessToken = await AuthenticateAccessTokenAsync(httpRequest, file, cancellationToken);

            if (authenticatedUserByCookie is null && authenticatedUserByAccessToken is null) return Forbidden("Could not authenticate either the auth cookie or the access_token (if provided)");

            if (authenticatedUserByCookie is null) return authenticatedUserByAccessToken;

            if (authenticatedUserByAccessToken is null) return authenticatedUserByCookie;

            if (authenticatedUserByCookie.Id == authenticatedUserByAccessToken.Id) return authenticatedUserByAccessToken;

            _logger?.LogCritical("ATTACK WARNING: Both the auth cookie '{AuthCookieUser}' and the access_token '{AccessTokenUser}' were provided, but they do not belong to the same user", authenticatedUserByCookie.Id, authenticatedUserByAccessToken.Id);

            return Forbidden("Both the auth cookie and the access_token were provided, but they do not belong to the same user");
        }

        async Task<UserFileAccessToken> IUserAuthenticationService.GenerateAccessToken(AuthenticatedUser authenticatedUser, File file, FileAccessPermission fileAccessPermission, CancellationToken cancellationToken)
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));

            var expiresAtUtc = _systemClock.UtcNow.AddHours(3);  // TODO - Could be a configuration setting

            var accessToken = await _userFileAccessTokenRepository.GenerateAsync(authenticatedUser, file, fileAccessPermission, expiresAtUtc, cancellationToken);

            return accessToken;
        }


        private async Task<AuthenticatedUser?> AuthenticateAccessTokenAsync(HttpRequest httpRequest, File file, CancellationToken cancellationToken)
        {
            var accessTokenQueryParameter = httpRequest.Query["access_token"].FirstOrDefault();

            _logger?.LogDebug("access_token query parameter set to '{AccessToken}'", accessTokenQueryParameter);

            if (string.IsNullOrWhiteSpace(accessTokenQueryParameter)) return Forbidden("There is no access_token query parameter associated with the request");

            if (!Guid.TryParse(accessTokenQueryParameter, out var accessToken)) return Forbidden("The access_token query parameter is not considered to be correctly formed");
            
            var userFileAccessToken = await _userFileAccessTokenRepository.GetAsync(accessToken, file, cancellationToken);

            Debug.Assert(userFileAccessToken.UserScope is not null);
            Debug.Assert(userFileAccessToken.UserScope.FileMetadata is not null);

            if (userFileAccessToken.ExpiresAtUtc <= _systemClock.UtcNow) return Forbidden($"The access token was recovered, but it expired at {userFileAccessToken.ExpiresAtUtc.ToString(CultureInfo.InvariantCulture)}");

            if (file != userFileAccessToken.UserScope.FileMetadata.AsFile()) return Forbidden("The file requested does not match to the scope of that bound to the access token");

            var authenticatedUser = userFileAccessToken.UserScope;

            var userFileMetadata = await _userFileMetadataProvider.GetForFileAsync(file, authenticatedUser, cancellationToken);

            return authenticatedUser with { FileMetadata = userFileMetadata };
        }

        private async Task<AuthenticatedUser?> AuthenticateCookieAsync(HttpRequest httpRequest, CancellationToken cancellationToken)
        {
            // If a user is logged into the system, we can defer to the web application to verify who they are and return to us the user
            // metadata that we are interested in.
            // At the time of writing, the user is assigned an authentication secret stored inside a cookie when they successfully log into the site.
            // This cookie is forwarded to all incoming requests (whether from Collabora or the Browser), but it is encrypted.   Rather than 
            // share encryption secrets with an old style ASP MVC application, simpler for now to call it, present the token and have it authenticate
            // while also renewing the cookie so it does not expire

            var requestCookies = httpRequest.Headers["Cookie"].AsEnumerable();

            var hasCookies = requestCookies.Any();

            if (!hasCookies) return Forbidden("There is no Cookie header attached to the request");

            var mvcForumUserInfoUrl = _appConfiguration.MvcForumUserInfoUrl;

            var httpClient = _httpClientFactory.CreateClient("mvcforum-userinfo");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, mvcForumUserInfoUrl);

            httpRequestMessage.Headers.Add("Accept", "application/json; charset=utf-8");

            httpRequestMessage.Headers.Add("Cookie", requestCookies);

            try
            {
                using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                _logger?.LogDebug("Status code of response from Forum App is '{StatusCode}'", httpResponseMessage.StatusCode);

                if (!httpResponseMessage.IsSuccessStatusCode) return Forbidden("The authenticate user request sent to the forum app returned a non-success status code");

                var httpContent = httpResponseMessage.Content;

                if (httpContent is null) return Forbidden("The forum app returned a success status code but did not include a body");

                var mediaType = httpContent.Headers.ContentType?.MediaType;

                _logger?.LogDebug("Media Type of Response from Forum App UserInfo = '{MediaType'", mediaType);

                if (!mediaType?.Contains("application/json") ?? true) return Forbidden("The forum app response is for a media type the File Server does not support");

                using var utf8JsonStream = await httpContent.ReadAsStreamAsync(cancellationToken);

                var authenticatedUser = await JsonSerializer.DeserializeAsync<AuthenticatedUser>(utf8JsonStream, cancellationToken: cancellationToken);

                if (authenticatedUser is null) return Forbidden("Unable to convert the json body of the response from the forum app into an AuthenticatedUser");

                return authenticatedUser;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Failed to connect to the MVCForum UserInfo endpoint to verify the user attached to the current request");
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "Failed to deserialize the JSON response from the MVCForum UserInfo endpoint");
            }

            return Forbidden();
        }

        private AuthenticatedUser? Forbidden(string? message = default)
        {
            _logger?.LogDebug("The current user (associated with the incoming request) could not be identified.  {Message}", message);

            return default;
        }
    }
}
