using System.Security.Claims;
using FutureNHS.Infrastructure.Models.GroupPages;

namespace FutureNHS.Api.DataAccess.Models.GroupUser
{
    public record GroupUser
    {
        public Group Group { get; init; }
    }
}
