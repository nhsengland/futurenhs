using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

public interface IDiscussionCommand
{
    Task CreateDiscussionAsync(DiscussionDto discussion, CancellationToken cancellationToken = default);
}