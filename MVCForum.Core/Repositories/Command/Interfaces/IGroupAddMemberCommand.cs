using MvcForum.Core.Models.GroupAddMember;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    public interface IGroupAddMemberCommand
    {
        Task<GroupAddMemberResponse> AddMemberToGroupAsync(MailAddress invitedUserMailAddress,
                                                           string invitedUserRoleName,
                                                           string addedByUsername,
                                                           string invitedToGroupSlug,
                                                           CancellationToken cancellationToken);
    }
}
