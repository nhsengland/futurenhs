namespace FutureNHS.Api.DataAccess.Models.Permissions
{
    public record GroupUserRole
    {
        public string RoleName { get; init; }
        public bool Approved { get; init; }
        public bool Rejected { get; init; }
        public bool Locked { get; init; }
        public bool Banned { get; init; }
    }
}
