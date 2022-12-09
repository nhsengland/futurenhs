// using FutureNHS.Api.DataAccess.DTOs;
// using FutureNHS.Api.DataAccess.Models.Group;
// using FutureNHS.Api.DataAccess.Models.Registration;
//
// namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;
//
// public interface IRegistrationCommand
// {
//
//     Task<(uint totalCount, IEnumerable<PlatformInvite>)> GetPlatformInvitesAsync(Guid userId, uint offset, uint limit, CancellationToken cancellationToken = default);
//     
//     Task<IEnumerable<PendingGroupMember>> GetPendingGroupInvitesAsync(Guid groupId, CancellationToken cancellationToken = default);
//
// }
