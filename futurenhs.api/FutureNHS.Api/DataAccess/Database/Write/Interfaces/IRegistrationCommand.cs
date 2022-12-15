using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IRegistrationCommand
{

    Task DeletePlatformInvite(Guid inviteId, byte[] rowVersion, CancellationToken cancellationToken = default);
    
    Task<PlatformInvite> GetPlatformInviteById(Guid inviteId, CancellationToken cancellationToken = default);

    
}
