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

    public sealed class GroupInviteService : IGroupInviteService
    {
        private readonly IGroupInviteRepository _inviteRepository;
        private readonly IGroupInviteCommand _inviteCommand;
        private readonly IRegistrationEmailService _emailService;

        public GroupInviteService(IGroupInviteRepository inviteRepository, 
                                  IGroupInviteCommand inviteCommand,
                                  IRegistrationEmailService emailService)
        {
            _inviteRepository = inviteRepository ?? throw new ArgumentNullException(nameof(inviteRepository));
            _inviteCommand = inviteCommand ?? throw new ArgumentNullException(nameof(inviteCommand));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            return _inviteRepository.GetInvitesForGroupAsync(groupId, cancellationToken);
        }

        public Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.GetInvitesForGroupAsync(mailAddress, cancellationToken);
        }

        public Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.GetInviteForGroupAsync(groupId, mailAddress, cancellationToken);
        }

        public Task<bool> InviteExistsForGroupAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.InviteExistsForGroupAsync(groupId, mailAddress, cancellationToken);
        }

        public Task<bool> InviteExistsForMailAddressAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.InviteExistsForMailAddressAsync(mailAddress, cancellationToken);
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

        public Task<bool> MemberExistsInGroupAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.GroupMemberExistsAsync(groupId, mailAddress, cancellationToken);
        }

        public Task<bool> MemberExistsInSystemAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            if (mailAddress is null)
            {
                throw new ArgumentNullException(nameof(mailAddress));
            }

            if (string.IsNullOrWhiteSpace(mailAddress.Address))
            {
                throw new ArgumentNullException(nameof(mailAddress.Address));
            }

            return _inviteRepository.MemberExistsAsync(mailAddress, cancellationToken);
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
