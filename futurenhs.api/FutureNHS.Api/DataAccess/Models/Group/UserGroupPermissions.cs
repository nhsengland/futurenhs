namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record UserGroupPermissions
    {
        public string MemberStatus { get; init; }
        public IEnumerable<string?> Permissions { get; set; }
    }
}
