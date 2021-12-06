using Moq;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Tests.Services.GroupAddMember
{
    public class GroupAddMemberServiceTests
    {
        private readonly Mock<IGroupAddMemberRepository> _mockGroupAddMemberRepository;
        private readonly Mock<IGroupAddMemberCommand> _mockGroupAddMemberCommand;
        private readonly IGroupAddMemberService _groupAddMemberService;
        public GroupAddMemberServiceTests()
        {
            _mockGroupAddMemberRepository = GroupAddMemberRepositoryMocks.GetGroupAddMemberRepository();
            _mockGroupAddMemberCommand = GroupAddMemberCommandMocks.GetGroupAddMemberCommand();

            _groupAddMemberService = new GroupAddMemberService(_mockGroupAddMemberRepository.Object, _mockGroupAddMemberCommand.Object);
        }

        #region GroupAddMemberQueryAsync Repository Tests

        [Test]
        public async Task Handle_ValidMailAddressValidGroupSlug_GroupAddMemberQueryAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedToGroupSlug = "group-slug";

            var response = await _groupAddMemberService.GroupAddMemberQueryAsync(invitedUserMailAddress, invitedToGroupSlug, CancellationToken.None);

            Assert.AreEqual(response.IsApproved, true);
            Assert.AreEqual(response.Response, ResponseType.Success);
        }

        [Test]
        public async Task Exception_InvalidGroupSlug_GroupAddMemberQueryAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedToGroupSlug = string.Empty;

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupAddMemberService.GroupAddMemberQueryAsync(invitedUserMailAddress, invitedToGroupSlug, CancellationToken.None));
            Assert.AreEqual(response.ParamName, nameof(invitedToGroupSlug));
        }

        [Test]
        public async Task Exception_InvalidMailAddress_GroupAddMemberQueryAsync()
        {
            MailAddress invitedUserMailAddress = null;
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupAddMemberService.GroupAddMemberQueryAsync(invitedUserMailAddress, invitedToGroupSlug, CancellationToken.None));
            Assert.AreEqual(response.ParamName, nameof(invitedUserMailAddress));
        }

        #endregion GroupAddMemberQueryAsync Repository Tests

        #region IsCurrentMemberAdminAsync Repository Tests

        [Test]
        public async Task Handle_ValidAdminValidGroupSlug_IsCurrentMemberAdminAsync()
        {
            var currentMemberUsername = "admin@email.com";
            var invitedToGroupSlug = "group-slug";

            var response = await _groupAddMemberService.IsCurrentMemberAdminAsync(currentMemberUsername, invitedToGroupSlug, CancellationToken.None);
            Assert.IsTrue(response);
        }

        [Test]
        public async Task Exception_InvalidAdmin_IsCurrentMemberAdminAsync()
        {
            var currentMemberUsername = string.Empty;
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupAddMemberService.IsCurrentMemberAdminAsync(currentMemberUsername, invitedToGroupSlug, CancellationToken.None));
            Assert.AreEqual(response.ParamName, nameof(currentMemberUsername));
        }

        [Test]
        public async Task Exception_InvalidGroupSlug_IsCurrentMemberAdminAsync()
        {
            var currentMemberUsername = "admin@email.com";
            var invitedToGroupSlug = string.Empty;

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupAddMemberService.IsCurrentMemberAdminAsync(currentMemberUsername, invitedToGroupSlug, CancellationToken.None));
            Assert.AreEqual(response.ParamName, nameof(invitedToGroupSlug));
        }

        #endregion IsCurrentMemberAdminAsync Repository Tests

        #region AddMemberToGroupAsync Command Tests

        [Test]
        public async Task Handles_ValidInput_AddMemberToGroupAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedUserRoleName = "Standard Members";
            var addedByUsername = "admin";
            var invitedToGroupSlug = "group-slug";

            var response = await _groupAddMemberService.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                              invitedUserRoleName,
                                                                              addedByUsername,
                                                                              invitedToGroupSlug,
                                                                              CancellationToken.None);

            Assert.AreEqual(response, ResponseType.Success);
        }

        [Test]
        public async Task Exception_InvalidMailAddress_AddMemberToGroupAsync()
        {
            MailAddress invitedUserMailAddress = null;
            var invitedUserRoleName = "Standard Members";
            var addedByUsername = "admin";
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                   invitedUserRoleName,
                                                                   addedByUsername,
                                                                   invitedToGroupSlug,
                                                                   CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(invitedUserMailAddress));
        }

        [Test]
        public async Task Exception_InvalidRoleName_AddMemberToGroupAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedUserRoleName = string.Empty;
            var addedByUsername = "admin";
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                   invitedUserRoleName,
                                                                   addedByUsername,
                                                                   invitedToGroupSlug,
                                                                   CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(invitedUserRoleName));
        }

        [Test]
        public async Task Exception_InvalidUsername_AddMemberToGroupAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedUserRoleName = "Standard Member";
            var addedByUsername = string.Empty;
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                   invitedUserRoleName,
                                                                   addedByUsername,
                                                                   invitedToGroupSlug,
                                                                   CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(addedByUsername));
        }

        [Test]
        public async Task Exception_InvalidGroupSlug_AddMemberToGroupAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var invitedUserRoleName = "Standard Member";
            var addedByUsername = "admin";
            var invitedToGroupSlug = string.Empty;

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.AddMemberToGroupAsync(invitedUserMailAddress,
                                                                   invitedUserRoleName,
                                                                   addedByUsername,
                                                                   invitedToGroupSlug,
                                                                   CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(invitedToGroupSlug));
        }

        #endregion AddMemberToGroupAsync Command Tests

        #region ApproveGroupMemberAsync Command Tests

        [Test]
        public async Task Handles_ValidInput_ApproveGroupMemberAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var approvedByUsername = "admin";
            var invitedToGroupSlug = "group-slug";

            var response = await _groupAddMemberService.ApproveGroupMemberAsync(invitedUserMailAddress,
                                                                                approvedByUsername,
                                                                                invitedToGroupSlug,
                                                                                CancellationToken.None);

            Assert.AreEqual(response, ResponseType.Success);
        }

        [Test]
        public async Task Exception_InvalidMailAddress_ApproveGroupMemberAsync()
        {
            MailAddress invitedUserMailAddress = null;
            var approvedByUsername = "admin";
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.ApproveGroupMemberAsync(invitedUserMailAddress,
                                                                     approvedByUsername,
                                                                     invitedToGroupSlug,
                                                                     CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(invitedUserMailAddress));
        }

        [Test]
        public async Task Exception_InvalidUsername_ApproveGroupMemberAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var approvedByUsername = string.Empty;
            var invitedToGroupSlug = "group-slug";

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.ApproveGroupMemberAsync(invitedUserMailAddress,
                                                                     approvedByUsername,
                                                                     invitedToGroupSlug,
                                                                     CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(approvedByUsername));
        }

        [Test]
        public async Task Exception_InvalidGroupSlug_ApproveGroupMemberAsync()
        {
            var invitedUserMailAddress = new MailAddress("user@email.com");
            var approvedByUsername = "admin";
            var invitedToGroupSlug = string.Empty;

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _groupAddMemberService.ApproveGroupMemberAsync(invitedUserMailAddress,
                                                                     approvedByUsername,
                                                                     invitedToGroupSlug,
                                                                     CancellationToken.None));

            Assert.AreEqual(response.ParamName, nameof(invitedToGroupSlug));
        }

        #endregion ApproveGroupMemberAsync Command Tests
    }
}
