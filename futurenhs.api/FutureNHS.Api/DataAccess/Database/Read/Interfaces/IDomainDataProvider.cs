using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IDomainDataProvider
    {
        Task<bool> IsDomainApprovedAsync(string emailDomain, CancellationToken cancellationToken = default);
        
        Task<bool> IsDomainDeletedAsync(string emailDomain, CancellationToken cancellationToken = default);

    }
}
