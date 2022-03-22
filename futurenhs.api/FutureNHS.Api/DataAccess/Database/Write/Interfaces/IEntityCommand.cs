namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IEntityCommand
    {
        Task CreateEntityAsync(Guid entityId, CancellationToken cancellationToken = default);
    }
}
