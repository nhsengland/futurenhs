using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IDomainCommand
{
    Task<DomainDto> GetDomainAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<DomainDto> GetDeletedDomainAsync(string emailDomain, CancellationToken cancellationToken = default);

    Task CreateApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
    Task DeleteApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
    
    Task RestoreApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);

    Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(uint offset, uint limit, CancellationToken cancellationToken = default);
}

