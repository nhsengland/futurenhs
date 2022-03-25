using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IDiscussionCommand
{
    Task CreateDiscussionAsync(DiscussionDto discussion, CancellationToken cancellationToken = default);
}