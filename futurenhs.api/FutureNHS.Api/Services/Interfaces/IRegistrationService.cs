﻿using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Models.Domain.Request;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member.Request;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task InviteMemberToGroupAndPlatformAsync(Guid userId, string groupSlug, string email, CancellationToken cancellationToken);
        
        Task InviteMemberToPlatformAsync(Guid userId, string email, CancellationToken cancellationToken);

        Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken);

        Task<MemberInfoResponse> MapMemberToIdentityAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken);

        Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken);   
        
        Task<Boolean> UpdateDomainAsync(string domain,CancellationToken cancellationToken);
        
        Task<Boolean> AddDomainAsync(RegisterDomainRequest domainRequest,CancellationToken cancellationToken);

    }
}