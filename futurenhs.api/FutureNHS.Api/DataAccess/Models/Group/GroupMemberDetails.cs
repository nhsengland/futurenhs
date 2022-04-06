namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record GroupMemberDetails : BaseData
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string Slug{ get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Initials { get; init; }
        public string Email { get; init; }
        public string Pronouns { get; init; }
        public string DateJoinedUtc { get; init; }
        public string LastLoginUtc { get; init; }
        public Guid RoleId { get; init; }
        public string Role { get; init; }
        public Image ProfileImage { get; init; }
    }
}
