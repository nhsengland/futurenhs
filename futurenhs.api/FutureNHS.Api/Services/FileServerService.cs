using System.Net;
using System.Security;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.Models.FileServer;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Headers;

namespace FutureNHS.Api.Services
{
    public sealed class FileServerService : IFileServerService
    {
        private const string ViewFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/view";
        private const string EditFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/edit";

        private readonly ILogger<FileServerService> _logger;
        private readonly string _fileServerPrimaryConnectionString;
        private readonly string _fileServerFilePlaceHolderId;
        private readonly string _fileServerPermissionPlaceHolderId;
        private readonly IPermissionsService _permissionsService;
        private readonly IHttpClientFactory _httpClientFactory;

        public FileServerService(IOptionsSnapshot<FileServerTemplateUrlStrings> fileServerTemplateUrlStrings, IHttpClientFactory httpClientFactory, IPermissionsService permissionsService, ILogger<FileServerService> logger)
        {
            if (fileServerTemplateUrlStrings is null) throw new ArgumentNullException(nameof(fileServerTemplateUrlStrings));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            _fileServerPrimaryConnectionString = fileServerTemplateUrlStrings.Value.TemplateUrl;
            _fileServerFilePlaceHolderId = fileServerTemplateUrlStrings.Value.TemplateUrlFileIdPlaceholder;
            _fileServerPermissionPlaceHolderId = fileServerTemplateUrlStrings.Value.TemplateUrlPermissionPlaceholder;
            _permissionsService = permissionsService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<FileServerCollaboraResponse?> GetCollaboraFileUrl(Guid userId, string slug, string permission, Guid file, string authHeader, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(permission)) throw new ArgumentNullException(nameof(permission));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            string fileAccess;
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditFileRole, cancellationToken);
            if (userCanPerformAction)
            {
                fileAccess = "edit";
            }
            else
            {
                userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, ViewFileRole, cancellationToken);
                if (userCanPerformAction is false)
                {
                    _logger.LogError($"Error: ViewFileAsync - User:{userId} does not have access to group:{slug}");
                    throw new SecurityException($"Error: User does not have access");
                }

                fileAccess = "view";
            }

            var fileRequestUrl = _fileServerPrimaryConnectionString
                .Replace(_fileServerFilePlaceHolderId, file.ToString())
                .Replace(_fileServerPermissionPlaceHolderId, fileAccess);
    
            
            var httpClient = _httpClientFactory.CreateClient("fileserver-createurl");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, fileRequestUrl);

            httpRequestMessage.Headers.Add("Accept", "application/json; charset=utf-8");

            httpRequestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

            try
            {
                using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                _logger?.LogDebug($"Status code of response from file server is '{httpResponseMessage.StatusCode}'");

                if (!httpResponseMessage.IsSuccessStatusCode) return Forbidden(httpResponseMessage.StatusCode, "The GenerateCollaboraURL request sent to the file server returned a non-success status code");

                var httpContent = httpResponseMessage.Content;

                var mediaType = httpContent.Headers.ContentType?.MediaType;

                _logger?.LogDebug($"Media Type of Response from file server FileServerCollaboraResponse = '{mediaType}");

                if (!mediaType?.Contains("application/json") ?? true) return Forbidden(httpResponseMessage.StatusCode, "The file server response is for a media type the File Server does not support");

                await using var utf8JsonStream = await httpContent.ReadAsStreamAsync(cancellationToken);

                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var fileServerResponse = await JsonSerializer.DeserializeAsync<FileServerCollaboraResponse>(utf8JsonStream, options, cancellationToken: cancellationToken);

                return fileServerResponse ?? Forbidden(httpResponseMessage.StatusCode, "Unable to convert the json body of the response from the file server into an FileServerCollaboraResponse");
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Failed to connect to the FileServer endpoint for the current request");
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "Failed to deserialize the JSON response from the FileServer endpoint");
            }

            return Forbidden(HttpStatusCode.InternalServerError);
        }

        private FileServerCollaboraResponse? Forbidden(HttpStatusCode? httpStatusCode, string? message = default)
        {
            _logger?.LogDebug("The current user (associated with the incoming request) could not be identified.  {Message}", message);
            throw new HttpRequestException("Error generating url", null, httpStatusCode);
        }
    }
}


