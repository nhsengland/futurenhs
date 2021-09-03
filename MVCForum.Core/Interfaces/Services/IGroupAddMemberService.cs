using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IGroupAddMemberService
    {
        Task<GroupAddMemberResponse> AddMemberToGroupAsync(MailAddress invitedUserMailAddress, 
                                                           string invitedUserRoleName, 
                                                           string addedByUsername, 
                                                           string invitedToGroupSlug, 
                                                           CancellationToken cancellationToken);

        Task<ResponseType> IsMemberMailAddressValidAsync(MailAddress invitedUserMailAddress,
                                                         string invitedToGroupSlug, 
                                                         CancellationToken cancellationToken);

        Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername,
                                             string invitedToGroupSlug,
                                             CancellationToken cancellationToken);
    }
}
