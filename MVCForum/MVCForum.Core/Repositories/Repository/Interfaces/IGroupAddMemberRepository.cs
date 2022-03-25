using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    public interface IGroupAddMemberRepository
    {
        Task<GroupAddMemberQueryResponse> GroupAddMemberQueryAsync(MailAddress invitedUserMailAddress,
                                                                   string invitedToGroupSlug,
                                                                   CancellationToken cancellationToken);

        Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername,
                                             string invitedToGroupSlug,
                                             CancellationToken cancellationToken);       
    }
}