using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IDomainDataProvider
    {
        Task<bool> IsDomainApprovedAsync(string emailDomain, CancellationToken cancellationToken = default);
        
        Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(uint offset, uint limit, CancellationToken cancellationToken = default);

    }
}
