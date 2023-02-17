namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record GroupInviteSummary
    {
        public Guid Id { get; init; }
        public string NameText { get; init; }
        public string StraplineText { get; init; }
        public int MemberCount { get; init; }
        public int DiscussionCount { get; init; }
        
        public int FileCount { get; init; }
        public string Slug { get; init; }
        public bool IsPublic { get; init; }
        public ImageData Image { get; init; }
        public GroupInvite Invite { get; init; }
        public Guid? ThemeId { get; init; }
    }
}
