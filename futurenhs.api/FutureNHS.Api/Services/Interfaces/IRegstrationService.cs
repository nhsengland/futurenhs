using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task InviteMemberToGroupAndPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken);

        Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken);
    }
}
