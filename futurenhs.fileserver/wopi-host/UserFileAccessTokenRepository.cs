using FutureNHS.WOPIHost.Azure;
using FutureNHS.WOPIHost.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IUserFileAccessTokenRepository
    {
        Task<UserFileAccessToken> GenerateAsync(AuthenticatedUser authenticatedUser, File file, FileAccessPermission fileAccessPermission, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken);
        Task<UserFileAccessToken> GetAsync(Guid accessToken, File file, CancellationToken cancellationToken);
    }

    public sealed class UserFileAccessTokenRepository : IUserFileAccessTokenRepository
    {
        private readonly IAzureTableStoreClient _azureTableStoreClient;
        private readonly string _azureTableName;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<UserFileAccessTokenRepository> _logger;

        public UserFileAccessTokenRepository(IAzureTableStoreClient azureTableStoreClient, IOptionsSnapshot<AzurePlatformConfiguration> configuration, ISystemClock systemClock, ILogger<UserFileAccessTokenRepository> logger)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var tableName = configuration.Value?.AzureTableStorage?.AccessTokenTableName;

            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentOutOfRangeException(nameof(configuration), "AccessTokenTableName is null");

            _azureTableName = tableName;
            _azureTableStoreClient = azureTableStoreClient ?? throw new ArgumentNullException(nameof(azureTableStoreClient));
            _systemClock = systemClock                     ?? throw new ArgumentNullException(nameof(systemClock));

            _logger = logger;
        }

        async Task<UserFileAccessToken> IUserFileAccessTokenRepository.GenerateAsync(AuthenticatedUser authenticatedUser, File file, FileAccessPermission fileAccessPermission, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken)
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (authenticatedUser.Id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(authenticatedUser));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));
            if (expiresAtUtc == DateTimeOffset.MinValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
            if (expiresAtUtc == DateTimeOffset.MaxValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
            if (authenticatedUser.FileMetadata is null) throw new ArgumentOutOfRangeException(nameof(authenticatedUser));
            if (file != authenticatedUser.FileMetadata.AsFile()) throw new ArgumentOutOfRangeException(nameof(authenticatedUser));

            cancellationToken.ThrowIfCancellationRequested();

            Debug.Assert(file.Id is not null);

            var id = Guid.NewGuid();

            var accessToken = new UserFileAccessToken(id, authenticatedUser, fileAccessPermission, expiresAtUtc);

            var serialisedAccessToken = JsonSerializer.Serialize(accessToken);

            var partitionKey = file.Id;
            var rowKey = id.ToString();

            var record = new Dictionary<string, object>() { 
                { "UserId", authenticatedUser.Id },
                { "FileId", file.Id },
                { "ExpiresAtUtc", expiresAtUtc },
                { "Token", serialisedAccessToken },
            };

            await _azureTableStoreClient.AddRecordAsync(_azureTableName, partitionKey, rowKey, record, cancellationToken);

            return accessToken;
        }

        async Task<UserFileAccessToken?> IUserFileAccessTokenRepository.GetAsync(Guid accessToken, File file, CancellationToken cancellationToken)
        {
            if (Guid.Empty == accessToken) throw new ArgumentNullException(nameof(accessToken));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));

            cancellationToken.ThrowIfCancellationRequested();

            Debug.Assert(file.Id is not null);

            var partitionKey = file.Id;
            var rowKey = accessToken.ToString();

            var record = await _azureTableStoreClient.ReadRecordAsync(_azureTableName, partitionKey, rowKey, cancellationToken);

            var serialisedAccessToken = record["Token"] as string;

            if (string.IsNullOrWhiteSpace(serialisedAccessToken)) return default;

            var deserialisedAccessToken = JsonSerializer.Deserialize<UserFileAccessToken>(serialisedAccessToken);

            return deserialisedAccessToken;
        }
    }
}
