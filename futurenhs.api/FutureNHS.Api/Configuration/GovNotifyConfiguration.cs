namespace FutureNHS.Api.Configuration
{
    public sealed class GovNotifyConfiguration
    {
        public string ApiKey { get; init; }
        public string RegistrationEmailTemplateId { get; init; }
        public string GroupRegistrationEmailTemplateId { get; init; }
        public string GroupInviteEmailTemplateId { get; init; }
        public string CommentOnDiscussionEmailTemplateId { get; init; }
        public string ResponseToCommentEmailTemplateId { get; init; }
        public string GroupMemberRequestRejectedEmailTemplateId { get; init; }
        public string GroupMemberRequestAcceptedEmailTemplateId { get; init; }
        public string GroupMemberRequestEmailTemplateId { get; init; }
    }
}
