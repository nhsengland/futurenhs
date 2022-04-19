using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Models.File;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupService
    {
        Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(Guid userId,
            uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize,
            CancellationToken cancellationToken = default);

        Task<GroupData?> GetGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);

        Task UpdateGroupMultipartDocument(Guid userId, string slug, byte[] rowVersion, Stream requestBody, string? contentType,
            CancellationToken cancellationToken);

    }
}
