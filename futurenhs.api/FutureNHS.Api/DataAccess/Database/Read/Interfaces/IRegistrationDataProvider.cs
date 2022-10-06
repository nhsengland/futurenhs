using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IRegistrationDataProvider
    {
        Task<InviteDetails> GetRegistrationInviteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
