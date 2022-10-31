using System.Text.Json;
using FileServer.Enums;
using FileServer.Models;
using FileServer.PlatformHelpers;
using FileServer.PlatformHelpers.Interfaces;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FileServer.DataAccess
{
    public sealed class UserFileAccessTokenRepository : IUserFileAccessTokenRepository
    {
        private readonly IAzureTableStoreClient _azureTableStoreClient;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<UserFileAccessTokenRepository> _logger;

        public UserFileAccessTokenRepository(IAzureTableStoreClient azureTableStoreClient, ISystemClock systemClock, ILogger<UserFileAccessTokenRepository> logger)
        {
            _azureTableStoreClient = azureTableStoreClient ?? throw new ArgumentNullException(nameof(azureTableStoreClient));
            _systemClock = systemClock                     ?? throw new ArgumentNullException(nameof(systemClock));

            _logger = logger;
        }

        public async Task<UserFileAccessToken> GenerateAsync(AuthenticatedUser authenticatedUser, Guid fileId, FileAccessPermission fileAccessPermission, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken)
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (authenticatedUser.Id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(authenticatedUser));
            if (expiresAtUtc == DateTimeOffset.MinValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
            if (expiresAtUtc == DateTimeOffset.MaxValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
            if (authenticatedUser.FileMetadata is null) throw new ArgumentOutOfRangeException(nameof(authenticatedUser));

            cancellationToken.ThrowIfCancellationRequested();

            var id = Guid.NewGuid();

            var accessToken = new UserFileAccessToken
            {
                Id = id, 
                User = authenticatedUser with { AccessToken = id}, 
                FileAccessPermission = fileAccessPermission, 
                ExpiresAtUtc = expiresAtUtc
            };

            var serialisedAccessToken = JsonSerializer.Serialize(accessToken);

            var tokenEntity = new AccessTokenEntity
            {
                PartitionKey = fileId.ToString(),
                RowKey = id.ToString(),
                UserId = authenticatedUser.Id.ToString(),
                FileId = fileId.ToString(),
                ExpiresAtUtc = expiresAtUtc,
                Token = serialisedAccessToken
            };

            await _azureTableStoreClient.AddEntity(tokenEntity, cancellationToken);

            return accessToken;
        }

        public async Task<UserFileAccessToken?> GetAsync(Guid accessToken, Guid fileId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == accessToken) throw new ArgumentNullException(nameof(accessToken));
            if (Guid.Empty == fileId) throw new ArgumentNullException(nameof(fileId));

            cancellationToken.ThrowIfCancellationRequested();

            var partitionKey = fileId.ToString();
            var rowKey = accessToken.ToString();

            var record = await _azureTableStoreClient.ReadEntityAsync(partitionKey, rowKey, cancellationToken);
            
            if (string.IsNullOrWhiteSpace(record.Token)) return default;

            var deserialisedAccessToken = JsonSerializer.Deserialize<UserFileAccessToken>(record.Token);

            return deserialisedAccessToken;
        }
    }
}
