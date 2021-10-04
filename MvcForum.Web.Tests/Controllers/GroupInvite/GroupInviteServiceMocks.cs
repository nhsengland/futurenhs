using Moq;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Repositories.Models;
using System;
using System.Net.Mail;
using System.Threading;

namespace MvcForum.Web.Tests.Controllers.GroupInvite
{
    public class GroupInviteServiceMocks
    {
        public static Mock<IGroupInviteService> GetGroupInviteService()
        {
            var mockGroupInviteService = new Mock<IGroupInviteService>();

            mockGroupInviteService.Setup(repo => repo.IsMemberAdminAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (string currentMemberUsername, CancellationToken cancellationToken) =>
                {
                    if (currentMemberUsername.ToLower() == "admin")
                    {
                        return true;
                    }
                    return false;
                });

            mockGroupInviteService.Setup(repo => repo.MemberExistsInGroupAsync(It.IsAny<Guid>(), It.IsAny<MailAddress>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (Guid groupId, MailAddress invitedUserMailAddress, CancellationToken cancellationToken) =>
                {
                    if (Guid.Empty != groupId || string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
                    {
                        return false;
                    }

                    if (invitedUserMailAddress.Address.ToLower() == "sitemember@email.com")
                    {
                        return false;
                    }

                    if (invitedUserMailAddress.Address.ToLower() == "groupmember@email.com")
                    {
                        return true;
                    }

                    return false;
                });

            mockGroupInviteService.Setup(repo =>
                repo.MemberExistsInSystemAsync(It.IsAny<MailAddress>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                    (MailAddress invitedUserMailAddress, CancellationToken cancellationToken) =>
                    {
                        if (string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
                        {
                            return false;
                        }

                        if (invitedUserMailAddress.Address.ToLower() == "sitemember@email.com")
                        {
                            return true;
                        }

                        if (invitedUserMailAddress.Address.ToLower() == "groupmember@email.com")
                        {
                            return true;
                        }

                        return false;
                    });

            mockGroupInviteService.Setup(repo =>
                repo.InviteExistsForGroupAsync(It.IsAny<Guid>(), It.IsAny<MailAddress>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                    (Guid groupId, MailAddress invitedUserMailAddress, CancellationToken cancellationToken) =>
                    {
                        if (string.IsNullOrWhiteSpace(invitedUserMailAddress.Address))
                        {
                            return false;
                        }

                        if (invitedUserMailAddress.Address.ToLower() == "invitedmember@email.com")
                        {
                            return true;
                        }

                        return false;
                    });

            mockGroupInviteService.Setup(repo =>
                repo.CreateInviteAsync(It.IsAny<GroupInviteViewModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                    (GroupInviteViewModel invite, CancellationToken cancellationToken) =>
                    {
                        return Guid.NewGuid();
                    });


            return mockGroupInviteService;
        }
    }
}
