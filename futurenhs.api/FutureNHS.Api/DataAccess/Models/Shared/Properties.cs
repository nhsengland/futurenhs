using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.Shared
{
    public record Properties
    {
        public DateTime? AtUtc { get; init; }

        public UserNavProperty? By { get; init; }
    }
}
