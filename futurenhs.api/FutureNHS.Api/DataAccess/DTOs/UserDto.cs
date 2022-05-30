using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record UserDto : BaseData
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public string CreatedAtUtc { get; init; }
        public string ModifiedAtUtc { get; init; }
        public string LastLoginDateUtc { get; init; }
        public string Slug { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Initials { get; init; }
    }
}
