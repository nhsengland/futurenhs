using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IDomainDataProvider
    {
        Task<WhitelistDomain> GetWhitelistedDomainAsync(string emailDomain, CancellationToken cancellationToken = default);
    }
}
