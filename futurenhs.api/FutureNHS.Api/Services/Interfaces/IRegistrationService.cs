using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Models.Member.Request;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task InviteMemberToGroupAndPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken);
        
        Task InviteMemberToPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken);


        Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken);
        

        Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken);    }
}
