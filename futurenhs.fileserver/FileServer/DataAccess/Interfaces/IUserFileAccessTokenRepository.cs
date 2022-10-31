using FileServer.Enums;
using FileServer.Models;
using File = System.IO.File;

namespace FutureNHS.WOPIHost.Interfaces;

public interface IUserFileAccessTokenRepository
{
    Task<UserFileAccessToken> GenerateAsync(AuthenticatedUser authenticatedUser, Guid fileId, FileAccessPermission fileAccessPermission, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken);
    Task<UserFileAccessToken> GetAsync(Guid accessToken, Guid fileId, CancellationToken cancellationToken);
}