using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record Properties
    {
        public string? AtUtc { get; init; }

        public UserNavProperty? By  { get; init; }
    }
}
