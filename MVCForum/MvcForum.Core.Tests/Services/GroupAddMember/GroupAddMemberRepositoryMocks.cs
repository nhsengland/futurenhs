using Moq;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System.Net.Mail;
using System.Threading;

namespace MvcForum.Core.Tests.Services.GroupAddMember
{
    public class GroupAddMemberRepositoryMocks
    {

        public static Mock<IGroupAddMemberRepository> GetGroupAddMemberRepository()
        {            
            var mockGroupAddMemberRepository = new Mock<IGroupAddMemberRepository>();

            mockGroupAddMemberRepository.Setup(repo => repo.IsCurrentMemberAdminAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (string currentMemberUsername, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    return true;
                });

            mockGroupAddMemberRepository.Setup(repo => repo.GroupAddMemberQueryAsync(It.IsAny<MailAddress>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (MailAddress invitedUserMailAddress, string invitedToGroupSlug, CancellationToken cancellationToken) =>
                {
                    return new GroupAddMemberQueryResponse(true, ResponseType.Success);
                });

            return mockGroupAddMemberRepository;
        }
    }
}
