﻿using System.Data;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Domain.Request;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Services.Admin;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Services
{
    public sealed class RegistrationService : IRegistrationService
    {
        private const string AddMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/add";

        private readonly string _fqdn;
        private readonly ILogger<AdminUserService> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;
        private readonly IGroupCommand _groupCommand;
        private readonly IDomainCommand _domainCommand;
        private readonly IDomainDataProvider _domainDataProvider;
        private readonly IRegistrationDataProvider _registrationDataProvider;
        private readonly IUserService _userService;
        // Notification template Ids
        private readonly string _registrationEmailId;
        private readonly string _defaultRole;


        public RegistrationService(ILogger<AdminUserService> logger,
            ISystemClock systemClock,
            IPermissionsService permissionsService, 
            IUserCommand userCommand,
            IEmailService emailService,
            IUserService userService,
            IGroupCommand groupCommand,
            IRegistrationDataProvider registrationDataProvider,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig,
            IDomainCommand domainCommand,
            IDomainDataProvider domainDataProvider,
            IOptionsMonitor<DefaultSettings> defaultSettings)
        {
            _groupCommand = groupCommand;
            _permissionsService = permissionsService;
            _registrationDataProvider = registrationDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userService = userService;
            _userCommand = userCommand;
            _emailService = emailService;
            _domainCommand = domainCommand;
            _domainDataProvider = domainDataProvider;
            _fqdn = gatewayConfig.Value.FQDN;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
            _defaultRole = defaultSettings.CurrentValue.DefaultRole ?? throw new ArgumentOutOfRangeException(nameof(defaultSettings.CurrentValue.DefaultRole));

        }
        
        public async Task InviteMemberToGroupAndPlatformAsync(Guid userId, string groupSlug, string email, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(groupSlug)) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AddMembersRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: InviteMemberToGroupAndPlatformAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException($"Email was not provided");
            }

            if (email.Length > 254)
            {
                throw new ArgumentOutOfRangeException($"Email must be less than 254 characters");
            }

            MailAddress emailAddress;
            try
            {
                emailAddress = new MailAddress(email);
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException($"Email is not in a valid format");
            }
            var groupId = await _groupCommand.GetGroupIdForSlugAsync(groupSlug, cancellationToken);
            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId

            };

            var userInviteId = await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            //TODO: Check user is on platform and add to group invites list
            var registrationLink = CreateRegistrationLink(userInviteId);
            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };
            
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }
        public async Task InviteMemberToPlatformAsync(Guid userId, string email, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AddMembersRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: InviteMemberToGroupAndPlatformAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException($"Email was not provided");
            }

            if (email.Length > 254)
            {
                throw new ArgumentOutOfRangeException($"Email must be less than 254 characters");
            }

            MailAddress emailAddress;
            try
            {
                emailAddress = new MailAddress(email);
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException($"Email is not in a valid format");
            }

            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId

            };

            var userInviteId = await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            var registrationLink = CreateRegistrationLink(userInviteId);
            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };
            
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }


        public async Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(registrationRequest.Subject)) throw new ArgumentNullException(nameof(registrationRequest.Subject));
            if (string.IsNullOrEmpty(registrationRequest.Email)) throw new ArgumentNullException(nameof(registrationRequest.Email));
            if (string.IsNullOrEmpty(registrationRequest.Issuer)) throw new ArgumentNullException(nameof(registrationRequest.Issuer));

            // TODO Work for determining if domain is on auto approve list
            var emailAddress = new MailAddress(registrationRequest.Email);
            var domain = emailAddress.Host;
            // Boolean isDomainAllowed = true;
            // try
            // {
            //     isDomainAllowed = await _domainDataProvider.IsDomainApproved(domain, cancellationToken);
            // }
            // catch (DBConcurrencyException ex)
            // {
            //     _logger.LogError(ex, $"Error: User domain not listed");
            //     throw;
            // }


            if (await _userService.IsMemberInvitedAsync(registrationRequest.Email, cancellationToken))
            {
                // todo validate user

                var member = new MemberDto
                {
                    FirstName = registrationRequest.FirstName,
                    Surname = registrationRequest.LastName,
                    Email = registrationRequest.Email,
                    CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                    AgreedToTerms = registrationRequest.Agreed
                };
                try
                {
                    return await _userCommand.RegisterUserAsync(member, registrationRequest.Subject, registrationRequest.Issuer, _defaultRole, cancellationToken);
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: Error registering new user");
                    throw;
                }
            }
            return null;
        }

        public async Task<MemberInfoResponse> MapMemberToIdentityAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.SubjectId)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.SubjectId));
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.EmailAddress)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.EmailAddress));
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.Issuer)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.Issuer));

            var memberInfo = await _userCommand.GetMemberInfoAsync(memberIdentityRequest.SubjectId, cancellationToken);
            if (memberInfo is not null)
            {
                throw new PreconditionFailedExeption("Precondition Failed: User already mapped to this identity");
            }

            var memberDetailsResponse = await _userCommand.GetMemberByEmailAsync(memberIdentityRequest.EmailAddress, cancellationToken); ;
            if (memberDetailsResponse is not null)
            {
                await _userCommand.MapIdentityToExistingUserAsync(memberDetailsResponse.Id, memberIdentityRequest.SubjectId, memberIdentityRequest.Issuer, cancellationToken);
                return await _userCommand.GetMemberInfoAsync(memberIdentityRequest.SubjectId, cancellationToken);
            }
            throw new NotFoundException("Could not find a user with this email to map the identity to");
        }

        public async Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            var invite = await _registrationDataProvider.GetRegistrationInviteAsync(id, cancellationToken);
            return invite;
        }

        public async Task<Boolean> UpdateDomainAsync(string domain, CancellationToken cancellationToken)
        {
            return true;
        }
        
        public async Task<Boolean> AddDomainAsync(RegisterDomainRequest domainRequest, CancellationToken cancellationToken)
        {
            try
            {
                var domainDto = new DomainDto
                {
                    Id = Guid.NewGuid(),
                    EmailDomain = domainRequest.EmailDomain
                };
                await _domainCommand.CreateApprovedDomainAsync(domainDto, cancellationToken);
                return true;
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: Error creating new domain");
                throw;
            }
        }

        private string CreateRegistrationLink(Guid userInviteId)
        {
            var registrationLink = $"{_fqdn}/auth/invited?id={userInviteId}";
            return registrationLink;
        }
    }
}