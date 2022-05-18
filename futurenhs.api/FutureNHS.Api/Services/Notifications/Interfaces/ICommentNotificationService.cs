namespace FutureNHS.Api.Services.Notifications.Interfaces
{
    public interface ICommentNotificationService
    {
        Task SendNotificationToDiscussionCreatorAsync(Guid posterId, Guid discussionId, CancellationToken cancellationToken);
        Task SendNotificationToCommentCreatorAsync(Guid posterId, Guid commentId, CancellationToken cancellationToken);
    }
}
