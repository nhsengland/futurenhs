using Moq;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Repositories.Command.Interfaces;
using System.Net.Mail;
using System.Threading;

namespace MvcForum.Core.Tests.Services.GroupAddMember
{
    public class GroupAddMemberCommandMocks
    {
        public static Mock<IGroupAddMemberCommand> GetGroupAddMemberCommand()
        {
            var mockGroupAddMemberCommand =  new Mock<IGroupAddMemberCommand>();

            mockGroupAddMemberCommand.Setup(repo => 
                repo.AddMemberToGroupAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (MailAddress invitedUserMailAddress, string invitedUserRoleName, string addedByUsername, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    return ResponseType.Success;
                });

            mockGroupAddMemberCommand.Setup(repo =>
                repo.ApproveGroupMemberAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (MailAddress invitedUserMailAddress, string approvedByUsername, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    return ResponseType.Success;
                });

            return mockGroupAddMemberCommand;
        }
    }
}
