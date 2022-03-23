using System.Net;
using System.Security;
using System.Text;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.Models.FileServer;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FutureNHS.Api.Services
{
    public sealed class FileServerService : IFileServerService
    {
        private const string ViewFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/view";

        private readonly ILogger<FileServerService> _logger;
        private readonly string _fileServerPrimaryConnectionString;
        private readonly string _fileServerFilePlaceHolderId;
        private readonly IPermissionsService _permissionsService;
        private readonly IHttpClientFactory _httpClientFactory;

        public FileServerService(IOptionsSnapshot<FileServerTemplateUrlStrings> fileServerTemplateUrlStrings, IHttpClientFactory httpClientFactory, IPermissionsService permissionsService, ILogger<FileServerService> logger)
        {
            if (fileServerTemplateUrlStrings is null) throw new ArgumentNullException(nameof(fileServerTemplateUrlStrings));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            _fileServerPrimaryConnectionString = fileServerTemplateUrlStrings.Value.TemplateUrl;
            _fileServerFilePlaceHolderId = fileServerTemplateUrlStrings.Value.TemplateUrlFileIdPlaceholder;
            _permissionsService = permissionsService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        //public async Task<FileServerCollaboraResponse> GetCollaboraFileUrl(Guid userId,string slug, Guid file, CookieContainer cookies, string permission, CancellationToken cancellationToken)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();

        //    if (string.IsNullOrWhiteSpace(permission))
        //        throw new ArgumentNullException(nameof(permission));

        //    var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, ViewFileRole, cancellationToken);
        //    if (userCanPerformAction is false)
        //    {
        //        _logger.LogError($"Error: ViewFileAsync - User:{0} does not have access to group:{1}", userId, slug);
        //        throw new SecurityException($"Error: User does not have access");
        //    }

        //    var fileRequestUrl = _fileServerPrimaryConnectionString.Replace(_fileServerFilePlaceHolderId, file.ToString());

        //    // Have to use Webrequest as we need to send the cookies in the cookie container,
        //    // will be retired the moment MVCForum is gone.
        //    var request = (HttpWebRequest)WebRequest.Create($"{fileRequestUrl}");
        //    var postData = "";

        //    var data = Encoding.ASCII.GetBytes(postData);

        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = data.Length;
        //    request.Timeout = 3000;
        //    request.Method = "POST";
        //    request.CookieContainer = cookies;

        //    // If required by the server, set the credentials.
        //    request.Credentials = CredentialCache.DefaultCredentials;


        //    await using (var stream = request.GetRequestStream())
        //    {
        //        await stream.WriteAsync(data, 0, data.Length, cancellationToken);
        //    }

        //    try
        //    {
        //        using var response = (HttpWebResponse)request.GetResponse();

        //        if (response.StatusCode != HttpStatusCode.OK)
        //            throw new HttpRequestException("Error generating url", null, response.StatusCode);

        //        await using var dataStream = response.GetResponseStream();
        //        // Open the stream using a StreamReader for easy access.
        //        using var reader = new StreamReader(dataStream ?? throw new InvalidOperationException());
        //        // Read the content.
        //        var responseFromServer = await reader.ReadToEndAsync();
        //        //var fileServerResponse = JsonConvert.DeserializeObject<FileServerCollaboraResponse>(responseFromServer);
        //        //return fileServerResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error generating Collabora url for file {0}", file);
        //        throw;
        //    }
        //}

        public async Task<FileServerCollaboraResponse?> GetCollaboraFileUrl(Guid userId, string slug, string permission, Guid file, HttpRequest httpRequest, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(permission)) throw new ArgumentNullException(nameof(permission));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, ViewFileRole, cancellationToken);
            if (userCanPerformAction is false)
            {
                _logger.LogError($"Error: ViewFileAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var fileRequestUrl = _fileServerPrimaryConnectionString.Replace(_fileServerFilePlaceHolderId, file.ToString());

            var requestCookies = httpRequest.Headers["Cookie"].AsEnumerable();

            var hasCookies = requestCookies.Any();

            if (!hasCookies) return Forbidden(HttpStatusCode.Forbidden,"There is no Cookie header attached to the request");


            var httpClient = _httpClientFactory.CreateClient("fileserver-createurl");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, fileRequestUrl);

            httpRequestMessage.Headers.Add("Accept", "application/json; charset=utf-8");

            httpRequestMessage.Headers.Add("Cookie", requestCookies);

            try
            {
                using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                _logger?.LogDebug("Status code of response from file server is '{StatusCode}'", httpResponseMessage.StatusCode);

                if (!httpResponseMessage.IsSuccessStatusCode) return Forbidden(httpResponseMessage.StatusCode, "The GenerateCollaboraURL request sent to the file server returned a non-success status code");

                var httpContent = httpResponseMessage.Content;

                var mediaType = httpContent.Headers.ContentType?.MediaType;

                _logger?.LogDebug("Media Type of Response from file server FileServerCollaboraResponse = '{MediaType'", mediaType);

                if (!mediaType?.Contains("application/json") ?? true) return Forbidden(httpResponseMessage.StatusCode, "The file server response is for a media type the File Server does not support");

                var fileServerResponse = await httpContent.ReadFromJsonAsync<FileServerCollaboraResponse>(cancellationToken: cancellationToken);
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


