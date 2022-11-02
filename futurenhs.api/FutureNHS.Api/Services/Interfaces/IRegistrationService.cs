using System.Net.Mail;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Domain;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Models.Domain.Request;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member.Request;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task InviteMemberToGroupAndPlatformAsync(Guid userId, string groupSlug, string email,
            CancellationToken cancellationToken);
        
        Task InviteMemberToPlatformAsync(Guid userId, string email, CancellationToken cancellationToken);

        Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken);

        Task<MemberInfoResponse> MapMemberToIdentityAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken);

        Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken);

        Task DeleteDomainAsync(Guid userId, Guid domainId, byte[] rowVersion, CancellationToken cancellationToken);

        Task AddDomainAsync(Guid userId, RegisterDomainRequest domainRequest,CancellationToken cancellationToken);

        Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(Guid userId, uint offset, uint limit, CancellationToken cancellationToken);

        Task<DomainDto> GetDomainAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    }
}
