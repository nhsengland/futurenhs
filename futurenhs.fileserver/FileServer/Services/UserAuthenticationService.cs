using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using FileServer.Enums;
using FileServer.Models;
using FileServer.Services.Interfaces;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FileServer.Services
{
    public sealed class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly ILogger<UserAuthenticationService>? _logger;
        private readonly ISystemClock _systemClock;
        private readonly AppConfiguration _appConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserFileMetadataService _userFileMetadataService;
        private readonly IUserFileAccessTokenRepository _userFileAccessTokenRepository;
        private readonly string _userInfoUrl;

        public UserAuthenticationService(IUserFileMetadataService userFileMetadataService, IUserFileAccessTokenRepository userFileAccessTokenRepository, IHttpClientFactory httpClientFactory, IOptionsSnapshot<AppConfiguration> appConfiguration, ISystemClock systemClock, ILogger<UserAuthenticationService>? logger)
        {
            _logger = logger;

            _systemClock = systemClock                                     ?? throw new ArgumentNullException(nameof(systemClock));

            _userFileAccessTokenRepository = userFileAccessTokenRepository ?? throw new ArgumentNullException(nameof(userFileAccessTokenRepository));
            _userFileMetadataService = userFileMetadataService             ?? throw new ArgumentNullException(nameof(userFileMetadataService));
            _httpClientFactory = httpClientFactory                         ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _appConfiguration = appConfiguration?.Value                    ?? throw new ArgumentNullException(nameof(appConfiguration));

            _userInfoUrl = _appConfiguration.UserInfoUrl;
        }

        public async Task<AuthenticatedUser> AuthenticateUser(Guid fileId, string? authHeader, Guid? accessToken, FileAccessPermission accessPermission, CancellationToken cancellationToken)
        {
            AuthenticatedUser? authenticatedUser = null;
            if (accessToken.HasValue)
                authenticatedUser = await AuthenticateAccessTokenAsync(accessToken.Value, fileId, accessPermission, cancellationToken);
            else if(!string.IsNullOrEmpty(authHeader))
            {
                authenticatedUser = await AuthenticateAuthHeaderAsync(authHeader, fileId, accessPermission, cancellationToken);
                var userFileMetadata = await _userFileMetadataService.GetForFileAsync(fileId, authenticatedUser, cancellationToken);
                authenticatedUser =  authenticatedUser with { FileMetadata = userFileMetadata };
                if (authenticatedUser != null)
                {
                    if (!string.IsNullOrEmpty(userFileMetadata.FileVersion))
                        accessPermission = FileAccessPermission.View;
                    
                    var token = await GenerateAccessToken(authenticatedUser, fileId, accessPermission, cancellationToken);
                    authenticatedUser = token.User;
                }
            }

            if (authenticatedUser is null)
            {
                return Forbidden("User is not allowed to access this file");
            }

            return authenticatedUser;
        }
        
        public async Task<UserFileAccessToken> GenerateAccessToken(AuthenticatedUser authenticatedUser, Guid file, FileAccessPermission fileAccessPermission, CancellationToken cancellationToken)
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if(Guid.Empty == file) throw new ArgumentNullException(nameof(file));
            
            var expiresAtUtc = _systemClock.UtcNow.AddHours(3);  // TODO - Could be a configuration setting

            var accessToken = await _userFileAccessTokenRepository.GenerateAsync(authenticatedUser, file, fileAccessPermission, expiresAtUtc, cancellationToken);

            return accessToken;
        }
        

        private async Task<AuthenticatedUser?> AuthenticateAccessTokenAsync(Guid accessToken, Guid file, FileAccessPermission accessPermission, CancellationToken cancellationToken)
        {
            var userFileAccessToken = await _userFileAccessTokenRepository.GetAsync(accessToken, file, cancellationToken);

            if (userFileAccessToken.ExpiresAtUtc <= _systemClock.UtcNow) return Forbidden($"The access token was recovered, but it expired at {userFileAccessToken.ExpiresAtUtc.ToString(CultureInfo.InvariantCulture)}");

            if (file != userFileAccessToken.User.FileMetadata.FileId) return Forbidden("The file requested does not match to the scope of that bound to the access token");

            var authenticatedUser = userFileAccessToken.User;
            authenticatedUser = authenticatedUser with {UserAccess = userFileAccessToken.FileAccessPermission};
            return authenticatedUser;
        }

        private async Task<AuthenticatedUser?> AuthenticateAuthHeaderAsync(string authHeader, Guid file, FileAccessPermission accessPermission, CancellationToken cancellationToken)
        {
            try
            { 
                var userInfoUrl = _appConfiguration.UserInfoUrl;
                var fileIdPlaceHolder = _appConfiguration.TemplateUrlFileIdPlaceholder;
                var permissionPlaceHolder = _appConfiguration.TemplateUrlPermissionPlaceholder;

                if (string.IsNullOrEmpty(authHeader)) return Forbidden("There is no Auth header attached to the request");

                var httpClient = _httpClientFactory.CreateClient("api-userinfo");

                var fileRequestUrl = userInfoUrl
                    .Replace(fileIdPlaceHolder, file.ToString())
                    .Replace(permissionPlaceHolder, accessPermission == FileAccessPermission.View? "view": "edit");
                
                httpClient.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authHeader);
                httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                
                var authenticatedUser = await httpClient.GetFromJsonAsync<AuthenticatedUser>(fileRequestUrl, cancellationToken);

                return authenticatedUser ?? Forbidden("Unable to convert the json body of the response from the Api into an AuthenticatedUser");
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogDebug($"http request exception {ex.Message}");
                _logger?.LogError(ex, "Failed to connect to the UserInfo endpoint to verify the user attached to the current request");
            }
            catch (JsonException ex)
            {
                _logger?.LogDebug($"json exception {ex.Message}");
                _logger?.LogError(ex, "Failed to deserialize the JSON response from the UserInfo endpoint");
            }
            catch (Exception ex)
            {
                _logger?.LogDebug($"exception {ex.Message}");
                _logger?.LogError(ex, "Exception");
            }

            return Forbidden();
        }

        private AuthenticatedUser? Forbidden(string? message = default)
        {
            _logger?.LogDebug($"The current user (associated with the incoming request) could not be identified. {message}");

            return default;
        }
    }
}
