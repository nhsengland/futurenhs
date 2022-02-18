using FutureNHS.Api.DataAccess.Models.Comment;

namespace FutureNHS.Api.DataAccess.Models.Discussions
{
    public record Discussion
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Slug { get; init; }
        public string Description { get; init; }
        public bool IsSticky { get; init; }
        public int Views { get; init; }
        public int? TotalComments { get; init; }
        public Shared.Properties FirstRegistered { get; init; }
        public CommentNavProperty LastComment  { get; init; }
        public UserDiscussionDetails CurrentUser { get; init; }
    }
}
