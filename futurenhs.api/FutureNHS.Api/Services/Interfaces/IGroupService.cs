using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupService
    {
        Task CreateGroupAsync(Guid userId, Stream requestBody, string? contentType, CancellationToken cancellationToken);

        Task<GroupData?> GetGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);

        Task UpdateGroupMultipartDocument(Guid userId, string slug, byte[] rowVersion, Stream requestBody, string? contentType,
            CancellationToken cancellationToken);

    }
}
