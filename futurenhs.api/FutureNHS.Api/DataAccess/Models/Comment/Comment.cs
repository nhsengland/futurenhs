namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public record Comment
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public int RepliesCount { get; init; }
        public int LikesCount { get; init; }
        public Shared.Properties FirstRegistered { get; init; }
        public UserCommentDetails CurrentUser { get; init; }
    }
}
