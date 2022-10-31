using FileServer.Enums;
using FileServer.Models;
using File = FileServer.Models.File;

namespace FileServer.Services.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<AuthenticatedUser> AuthenticateUser(Guid fileId, string? authHeader, Guid? accessToken, FileAccessPermission accessPermission, CancellationToken cancellationToken);
    }
}
