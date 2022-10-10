using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IDomainCommand
{
    Task CreateApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
    Task DeleteApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
}

