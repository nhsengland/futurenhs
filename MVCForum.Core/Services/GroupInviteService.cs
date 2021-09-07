namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    public class GroupInviteService : IGroupInviteService
    {
        private readonly IGroupInviteRepository _inviteRepository;
        private readonly IGroupInviteCommand _inviteCommand;
        private readonly IRegistrationEmailService _emailService;

        public GroupInviteService(IGroupInviteRepository inviteRepository, 
                                  IGroupInviteCommand inviteCommand,
                                  IRegistrationEmailService emailService)
        {
            _inviteRepository = inviteRepository;
            _inviteCommand = inviteCommand;
            _emailService = emailService;
        }

        public Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            return _inviteRepository.GetInvitesForGroupAsync(groupId, cancellationToken);
        }

        public Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(string emailAddress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return _inviteRepository.GetInvitesForGroupAsync(emailAddress, cancellationToken);
        }

        public Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return _inviteRepository.GetInviteForGroupAsync(groupId, emailAddress, cancellationToken);
        }

        public Task<bool> InviteExistsForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return _inviteRepository.InviteExistsForGroupAsync(groupId, emailAddress, cancellationToken);
        }

        public async Task<Guid> CreateInviteAsync(GroupInviteViewModel model, CancellationToken cancellationToken)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                throw new ArgumentNullException(nameof(model.EmailAddress));
            }
            
            MailAddress to = new MailAddress(model.EmailAddress);
                        
            if (!await _emailService.SendInvitationAsync(to, cancellationToken))
            {
                return Guid.Empty;
            }

            return await _inviteCommand.CreateInviteAsync(model, cancellationToken);
        }

        public Task<bool> DeleteInviteAsync(Guid inviteId, CancellationToken cancellationToken)
        {
            if (inviteId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(inviteId));
            }

            return _inviteCommand.DeleteInviteAsync(inviteId, cancellationToken);
        }

        public Task<bool> MemberExistsInGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return _inviteRepository.GroupMemberExistsAsync(groupId, emailAddress, cancellationToken);
        }

        public Task<bool> MemberExistsInSystemAsync(string emailAddress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return _inviteRepository.MemberExistsAsync(emailAddress, cancellationToken);
        }

        public Task<bool> IsMemberAdminAsync(string username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            return _inviteRepository.IsMemberAdminAsync(username, cancellationToken);
        }
    }
}
