namespace FutureNHS.Api.Services.Notifications.Interfaces
{
    public interface IGroupMemberNotificationService
    {
        Task SendAcceptNotificationToMemberAsync(Guid membershipUserId, string groupName, CancellationToken cancellationToken);
        Task SendRejectNotificationToMemberAsync(Guid membershipUserId, string groupName, CancellationToken cancellationToken);
    }
}
