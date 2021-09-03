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
            if (groupAddMemberRepository is null)
            {
                throw new ArgumentNullException(nameof(groupAddMemberRepository));
            }

            if (groupAddMemberCommand is null)
            {
                throw new ArgumentNullException(nameof(groupAddMemberCommand));
            }

            _groupAddMemberRepository = groupAddMemberRepository;
            _groupAddMemberCommand = groupAddMemberCommand;
        }

        public Task<GroupAddMemberResponse> AddMemberToGroupAsync(MailAddress invitedUserMailAddress, 
                                                                        string invitedUserRoleName, 
                                                                        string addedByUsername, 
                                                                        string invitedToGroupSlug, 
                                                                        CancellationToken cancellationToken)
        {            
            return _groupAddMemberCommand.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                      invitedUserRoleName,
                                                                      addedByUsername,
                                                                      invitedToGroupSlug,
                                                                      cancellationToken);
        }
        
        public Task<ResponseType> IsMemberMailAddressValidAsync(MailAddress invitedUserMailAddress, string invitedToGroupSlug, CancellationToken cancellationToken)
        {
            return _groupAddMemberRepository.IsMemberMailAddressValidAsync(invitedUserMailAddress, invitedToGroupSlug, cancellationToken);
        }

        public Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername, string invitedToGroupSlug, CancellationToken cancellationToken)
        {
            return _groupAddMemberRepository.IsCurrentMemberAdminAsync(currentMemberUsername, invitedToGroupSlug, cancellationToken);
        }
    }
}
