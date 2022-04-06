using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.Models.User
{
    public sealed record Member : BaseData
    {
        public string Slug { get; init; }
        public string Name { get; init; }
        public string DateJoinedUtc { get; init; }
        public string LastLoginUtc { get; init; }
        public string Role { get; init; }
    }
}
