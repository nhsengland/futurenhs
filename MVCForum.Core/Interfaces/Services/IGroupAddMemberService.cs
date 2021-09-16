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
        Task<ResponseType> AddMemberToGroupAsync(MailAddress invitedUserMailAddress, 
                                                 string invitedUserRoleName, 
                                                 string addedByUsername, 
                                                 string invitedToGroupSlug, 
                                                 CancellationToken cancellationToken);

        Task<ResponseType> ApproveGroupMemberAsync(MailAddress invitedUserMailAddress,
                                                   string approvedByUsername,
                                                   string invitedToGroupSlug,
                                                   CancellationToken cancellationToken);

        Task<GroupAddMemberQueryResponse> GroupAddMemberQueryAsync(MailAddress invitedUserMailAddress,
                                                                   string invitedToGroupSlug, 
                                                                   CancellationToken cancellationToken);

        Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername,
                                             string invitedToGroupSlug,
                                             CancellationToken cancellationToken);
    }
}
