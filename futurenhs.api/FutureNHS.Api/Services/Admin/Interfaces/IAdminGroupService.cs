namespace FutureNHS.Api.Services.Admin.Interfaces
{
    public interface IAdminGroupService
    {
        Task CreateGroupAsync(Guid userId, Stream requestBody, string? contentType, CancellationToken cancellationToken);
    }
}
