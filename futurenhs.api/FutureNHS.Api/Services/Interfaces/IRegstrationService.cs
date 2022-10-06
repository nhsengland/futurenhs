namespace FutureNHS.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task InviteMemberToGroupAndPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken);

    }
}
