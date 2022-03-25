using Moq;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using System.Net.Mail;
using System.Threading;

namespace MvcForum.Web.Tests.Controllers.GroupInvite
{
    public class GroupAddMemberServiceMocks
    {
        public static Mock<IGroupAddMemberService> GetGroupAddMemberService()
        {
            var mockGroupAddMemberService = new Mock<IGroupAddMemberService>();           

            mockGroupAddMemberService.Setup(repo => repo.IsCurrentMemberAdminAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (string currentMemberUsername, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    if (currentMemberUsername.ToLower() == "admin")
                    {
                        return true;
                    }
                    return false;
                });

            mockGroupAddMemberService.Setup(repo => repo.GroupAddMemberQueryAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (MailAddress invitedUserMailAddress, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    if (invitedUserMailAddress.Address.ToLower() == "groupmember@email.com")
                    {
                        return new GroupAddMemberQueryResponse(true, ResponseType.AlreadyExists);
                    }

                    if (invitedUserMailAddress.Address.ToLower() == "sitemember@email.com")
                    {
                        return new GroupAddMemberQueryResponse(false, ResponseType.AlreadyExists);
                    }

                    if (invitedUserMailAddress.Address.ToLower() == "valid@email.com")
                    {
                        return new GroupAddMemberQueryResponse(false, ResponseType.Success);
                    }

                    return new GroupAddMemberQueryResponse(false, ResponseType.DoesntExist);
                });

            mockGroupAddMemberService.Setup(repo => 
                repo.ApproveGroupMemberAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                    (MailAddress invitedUserMailAddress, string approvedByUsernamre, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                    {
                        return ResponseType.Success;
                    });

            mockGroupAddMemberService.Setup(repo =>
                repo.AddMemberToGroupAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                    (MailAddress invitedUserMailAddress, string invitedUserRoleName, string addedByUsername, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                    {
                        return ResponseType.Success;
                    });



            return mockGroupAddMemberService;
        }        
    }
}
