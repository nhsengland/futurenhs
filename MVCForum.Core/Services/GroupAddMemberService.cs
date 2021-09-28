using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Services
{
    public sealed class GroupAddMemberService : IGroupAddMemberService
    {
        private readonly IGroupAddMemberRepository _groupAddMemberRepository;
        private readonly IGroupAddMemberCommand _groupAddMemberCommand;

        public GroupAddMemberService(IGroupAddMemberRepository groupAddMemberRepository,
                                     IGroupAddMemberCommand groupAddMemberCommand)
        {
            _groupAddMemberRepository = groupAddMemberRepository ?? throw new ArgumentNullException(nameof(groupAddMemberRepository));
            _groupAddMemberCommand = groupAddMemberCommand ?? throw new ArgumentNullException(nameof(groupAddMemberCommand));
        }

        public Task<ResponseType> AddMemberToGroupAsync(MailAddress invitedUserMailAddress, 
                                                        string invitedUserRoleName, 
                                                        string addedByUsername, 
                                                        string invitedToGroupSlug, 
                                                        CancellationToken cancellationToken)
        {
            if (invitedUserMailAddress is null)
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress));
            }

            if (string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress.Address));
            }

            if (string.IsNullOrWhiteSpace(invitedUserRoleName))
            {
                throw new ArgumentNullException(nameof(invitedUserRoleName));
            }

            if (string.IsNullOrWhiteSpace(addedByUsername))
            {
                throw new ArgumentNullException(nameof(addedByUsername));
            }

            if (string.IsNullOrWhiteSpace(invitedToGroupSlug))
            {
                throw new ArgumentNullException(nameof(invitedToGroupSlug));
            }

            return _groupAddMemberCommand.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                invitedUserRoleName,
                                                                addedByUsername,
                                                                invitedToGroupSlug,
                                                                cancellationToken);
        }

        public Task<ResponseType> ApproveGroupMemberAsync(MailAddress invitedUserMailAddress, 
                                                          string approvedByUsername, 
                                                          string invitedToGroupSlug, 
                                                          CancellationToken cancellationToken)
        {
            if (invitedUserMailAddress is null)
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress));
            }

            if (string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress.Address));
            }

            if (string.IsNullOrWhiteSpace(approvedByUsername))
            {
                throw new ArgumentNullException(nameof(approvedByUsername));
            }

            if (string.IsNullOrWhiteSpace(invitedToGroupSlug))
            {
                throw new ArgumentNullException(nameof(invitedToGroupSlug));
            }

            return _groupAddMemberCommand.ApproveGroupMemberAsync(invitedUserMailAddress,
                                                                  approvedByUsername,
                                                                  invitedToGroupSlug,
                                                                  cancellationToken);
        }

        public Task<GroupAddMemberQueryResponse> GroupAddMemberQueryAsync(MailAddress invitedUserMailAddress, string invitedToGroupSlug, CancellationToken cancellationToken)
        {
            if (invitedUserMailAddress is null)
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress));
            }

            if (string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
            {
                throw new ArgumentNullException(nameof(invitedUserMailAddress.Address));
            }

            if (string.IsNullOrWhiteSpace(invitedToGroupSlug))
            {
                throw new ArgumentNullException(nameof(invitedToGroupSlug));
            }

            return _groupAddMemberRepository.GroupAddMemberQueryAsync(invitedUserMailAddress, invitedToGroupSlug, cancellationToken);
        }

        public Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername, string invitedToGroupSlug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(currentMemberUsername))
            {
                throw new ArgumentNullException(currentMemberUsername);
            }

            if (string.IsNullOrWhiteSpace(invitedToGroupSlug))
            {
                throw new ArgumentNullException(currentMemberUsername);
            }

            return _groupAddMemberRepository.IsCurrentMemberAdminAsync(currentMemberUsername, invitedToGroupSlug, cancellationToken);
        }
    }
}
