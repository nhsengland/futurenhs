using Moq;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using MvcForum.Core.Repositories.Models;
using MvcForum.Web.Controllers;
using MvcForum.Web.Tests.Controllers.Base;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcForum.Web.Tests.Controllers.GroupInvite
{
    public class GroupInviteControllerTests
    {
        private readonly Mock<IGroupInviteService> _mockGroupInviteService;
        private readonly Mock<IGroupAddMemberService> _mockGroupAddMemberService;

        private static readonly Guid GroupIdValid = Guid.Parse("F3E6F8626A574200971F1ED3AAB438BE");
        private static readonly Guid GroupIdInvalid = Guid.Empty;
        private const string GroupSlugValid = "group-slug";
        private const string EmailValid = "valid@email.com";
        private const string EmailInvalid = "invalid@email.com";
        private const string ConfimEmailInvalid = "invalidconfirm@email.com";

        public GroupInviteControllerTests()
        {
            _mockGroupInviteService = GroupInviteServiceMocks.GetGroupInviteService();
            _mockGroupAddMemberService = GroupAddMemberServiceMocks.GetGroupAddMemberService();
        }

        #region GroupInviteMember Tests

        [Test]
        public async Task Handles_ValidGetRequest_InviteMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("admin");

            var result = await controller.InviteMemberAsync(GroupSlugValid, GroupIdValid, false, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<GroupInviteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, (result.Model as GroupInviteViewModel).Slug);
            Assert.AreEqual(GroupIdValid, (result.Model as GroupInviteViewModel).GroupId);
        }

        [Test]
        public async Task Handles_InvalidGetRequest_InviteMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("user");

            var result = await controller.InviteMemberAsync(GroupSlugValid, GroupIdValid, false, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", (result as RedirectToRouteResult).RouteName);
        }

        [Test]
        public async Task Handles_ValidPostRequest_InviteMemberAsync()
        {
            var groupInviteViewModel = new GroupInviteViewModel()
            {
                EmailAddress = EmailValid,
                ConfirmEmailAddress = EmailValid,
                GroupId = GroupIdValid,
                Slug = GroupSlugValid
            };
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("admin");

            var result = await controller.InviteMemberAsync(groupInviteViewModel, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual(GroupSlugValid, (result as RedirectToRouteResult).RouteValues["slug"]);
            Assert.AreEqual(GroupIdValid, (result as RedirectToRouteResult).RouteValues["groupId"]);
            Assert.AreEqual(true, (result as RedirectToRouteResult).RouteValues["success"]);
        }

        [Test]
        public async Task Handles_InvalidPostRequest_InviteMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("admin");
            controller.ModelState.AddModelError("error", "invalid state");
            var groupInviteViewModel = new GroupInviteViewModel()
            {
                EmailAddress = EmailInvalid,
                ConfirmEmailAddress = ConfimEmailInvalid,
                GroupId = GroupIdInvalid,
                Slug = GroupSlugValid
            };
            
            var result = await controller.InviteMemberAsync(groupInviteViewModel, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<GroupInviteViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, (result.Model as GroupInviteViewModel).Slug);
            Assert.AreEqual(GroupIdInvalid, (result.Model as GroupInviteViewModel).GroupId);
            Assert.AreEqual(EmailInvalid, (result.Model as GroupInviteViewModel).EmailAddress);
            Assert.AreEqual(ConfimEmailInvalid, (result.Model as GroupInviteViewModel).ConfirmEmailAddress);
        }

        #endregion GroupInviteMember Tests

        #region GroupAddMemberTests 

        [Test]
        public async Task Handles_ValidGetRequest_AddMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("admin");

            var result = await controller.AddMemberAsync(GroupSlugValid, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<GroupAddMemberViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, (result.Model as GroupAddMemberViewModel).Slug);
        }

        [Test]
        public async Task Handles_InvalidGetRequest_AddMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("user");

            var result = await controller.AddMemberAsync(GroupSlugValid, CancellationToken.None);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("GroupUrls", (result as RedirectToRouteResult).RouteName);
        }

        [Test]
        public async Task Handles_ValidPostRequest_AddMemberAsync()
        {
            var groupAddMemberViewModel = new GroupAddMemberViewModel()
            {
                Email = EmailValid,
                Slug = GroupSlugValid
            };
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            SetUserInContext.SetContext("admin");

            var result = await controller.AddMemberAsync(groupAddMemberViewModel, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<GroupAddMemberViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, (result.Model as GroupAddMemberViewModel).Slug);
            Assert.AreEqual(ResponseType.Success, (result.Model as GroupAddMemberViewModel).Response);
        }

        [Test]
        public async Task Handles_InvalidPostRequest_AddMemberAsync()
        {
            var controller = new GroupInviteController(_mockGroupInviteService.Object, _mockGroupAddMemberService.Object);
            controller.ModelState.AddModelError("error", "invalid state");
            SetUserInContext.SetContext("admin");
            var groupAddMemberViewModel = new GroupAddMemberViewModel()
            {
                Email = EmailInvalid,
                Slug = GroupSlugValid
            };

            var result = await controller.AddMemberAsync(groupAddMemberViewModel, CancellationToken.None) as ViewResult;

            Assert.IsInstanceOf<ActionResult>(result);
            Assert.IsInstanceOf<GroupAddMemberViewModel>(result.Model);
            Assert.AreEqual(GroupSlugValid, (result.Model as GroupAddMemberViewModel).Slug);
            Assert.AreEqual(EmailInvalid, (result.Model as GroupAddMemberViewModel).Email);
            Assert.AreEqual("invalid state", result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage);
        }

        #endregion GroupAddMemberTests
    }
}
