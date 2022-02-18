using System.Security.Claims;
using FutureNHS.Api.Models.Discussion;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IDiscussionService
    {
        Task CreateDiscussionAsync(Guid userId, string slug, Discussion discussion, CancellationToken cancellationToken);
    }
}
