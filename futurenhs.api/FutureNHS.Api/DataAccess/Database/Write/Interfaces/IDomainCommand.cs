using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IDomainCommand
{
    Task CreateWhitelistDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
    Task DeleteWhitelistDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken);
}

