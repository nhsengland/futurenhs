using FutureNHS.Api.DataAccess.Database.Read;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.Permissions;
using FutureNHS.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.Tests.Services
{
    [TestClass]
    public sealed class PermissionsServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermissionsService_GetUserPermissionsForGroupAsync_ThrowsIfRolesProviderNull()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            _ = new PermissionsService(null, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermissionsService_GetUserPermissionsForGroupAsync_ThrowsIfPermissionsDataProviderNull()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var rolesDataProvider = new Moq.Mock<IRolesDataProvider>().Object;

            _ = new PermissionsService(rolesDataProvider, null, permissionsServiceLogger, groupDataProvider);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ThrowsIfUserIdEmpty()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cancellationToken = CancellationToken.None;
            var rolesDataProvider = new Moq.Mock<IRolesDataProvider>().Object;
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var permissionsService = new PermissionsService(rolesDataProvider, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var userId = Guid.Empty;
            var groupId = Guid.NewGuid();

            _ = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ThrowsIfGroupIdEmpty()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cancellationToken = CancellationToken.None;
            var rolesDataProvider = new Mock<IRolesDataProvider>().Object;
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var permissionsService = new PermissionsService(rolesDataProvider, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var userId = Guid.NewGuid();
            var groupId = Guid.Empty;

            _ = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ThrowsIfCancelled()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var rolesDataProvider = new Mock<IRolesDataProvider>().Object;
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            cts.Cancel();

            var permissionsService = new PermissionsService(rolesDataProvider, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            _ = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);
        }


        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsGuestIfUserNotFound()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            List<string>? userRolesResponse = new List<string>();
            List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole>();

            rolesDataProvider.Setup(x =>
                    x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
                    .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);
            Assert.IsTrue(permissions is null);
        }

        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsGuestIfUserNotMemberOfGroup()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            List<string>? userRolesResponse = new List<string> { "Admin" };
            List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole>();

            rolesDataProvider.Setup(x =>
                    x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
                .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

            Assert.IsTrue(permissions.Any(x => x == "https://schema.collaborate.future.nhs.uk/groups/v1/members/add"));
        }

        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsGroupAdminSiteUser()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            List<string>? userRolesResponse = new List<string> { "Standard Members" };
            List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Admin", Approved = true } };

            rolesDataProvider.Setup(x =>
                    x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
                .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

            Assert.IsTrue(permissions.Any(x => x == "https://schema.collaborate.future.nhs.uk/groups/v1/members/add"));
        }

        //[TestMethod]
        //public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsPendingSiteUserAsGuest()
        //{
        //    var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
        //    var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;

        //    var cts = new CancellationTokenSource();

        //    var cancellationToken = cts.Token;

        //    var userId = Guid.NewGuid();
        //    var groupId = Guid.NewGuid();

        //    var rolesDataProvider = new Mock<IRolesDataProvider>();
        //    IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

        //    List<string>? userRolesResponse = new List<string> { "Standard Members" };
        //    List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Standard Members", Approved = false } };

        //    rolesDataProvider.Setup(x =>
        //            x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
        //        .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


        //    var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

        //    var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

        //    Assert.IsTrue(!permissions.Any());
        //}

        //[TestMethod]
        //public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsBannedSiteUserAsGuest()
        //{
        //    var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
        //    var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;

        //    var cts = new CancellationTokenSource();

        //    var cancellationToken = cts.Token;

        //    var userId = Guid.NewGuid();
        //    var groupId = Guid.NewGuid();

        //    var rolesDataProvider = new Mock<IRolesDataProvider>();
        //    IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

        //    List<string>? userRolesResponse = new List<string> { "Standard Members" };
        //    List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Standard Members", Approved = false, Banned = true } };

        //    rolesDataProvider.Setup(x =>
        //            x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
        //        .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


        //    var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

        //    var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

        //    Assert.IsTrue(!permissions.Any());
        //}

        //[TestMethod]
        //public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsLockedSiteUserAsGuest()
        //{
        //    var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
        //    var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;

        //    var cts = new CancellationTokenSource();

        //    var cancellationToken = cts.Token;

        //    var userId = Guid.NewGuid();
        //    var groupId = Guid.NewGuid();

        //    var rolesDataProvider = new Mock<IRolesDataProvider>();
        //    IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

        //    List<string>? userRolesResponse = new List<string> { "Standard Members" };
        //    List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Standard Members", Approved = true, Locked = true } };

        //    rolesDataProvider.Setup(x =>
        //            x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
        //        .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


        //    var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

        //    var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

        //    Assert.IsTrue(!permissions.Any());
        //}

        //[TestMethod]
        //public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsRejectedSiteUserAsGuest()
        //{
        //    var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
        //    var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;

        //    var cts = new CancellationTokenSource();

        //    var cancellationToken = cts.Token;

        //    var userId = Guid.NewGuid();
        //    var groupId = Guid.NewGuid();

        //    var rolesDataProvider = new Mock<IRolesDataProvider>();
        //    IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

        //    List<string>? userRolesResponse = new List<string> { "Standard Members" };
        //    List<GroupUserRole>? groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Standard Members", Approved = true, Rejected = true } };

        //    rolesDataProvider.Setup(x =>
        //            x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
        //        .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


        //    var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

        //    var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

        //    Assert.IsTrue(!permissions.Any());
        //}

        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsMultiplePermissionsForUserAndGroup()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var userRolesResponse = new List<string> { "Standard Members", "Admin" };
            var groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Admin", Approved = true }, new GroupUserRole { RoleName = "Standard Members", Approved = true } };

            rolesDataProvider.Setup(x =>
                    x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
                .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

            Assert.IsTrue(permissions.Any(x => x == "https://schema.collaborate.future.nhs.uk/members/v1/add"));
            Assert.IsTrue(permissions.Any(x => x == "https://schema.collaborate.future.nhs.uk/groups/v1/members/delete"));

        }

        //[TestMethod]
        //public async Task PermissionsService_GetUserPermissionsForGroupAsync_ReturnsBannedGroupUserWithMultipleRolesAsBanned()
        //{
        //    var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
        //    var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;

        //    var cts = new CancellationTokenSource();

        //    var cancellationToken = cts.Token;

        //    var userId = Guid.NewGuid();
        //    var groupId = Guid.NewGuid();

        //    var rolesDataProvider = new Mock<IRolesDataProvider>();
        //    IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

        //    var userRolesResponse = new List<string> { "Standard Members" };
        //    var groupRolesResponse = new List<GroupUserRole> { new GroupUserRole { RoleName = "Admin", Banned = true }, new GroupUserRole { RoleName = "Standard Members", Approved = true, Banned = true } };

        //    rolesDataProvider.Setup(x =>
        //            x.GetUserAndGroupUserRolesAsync(userId, groupId, cancellationToken))
        //        .ReturnsAsync(new UserAndGroupRoles(userRolesResponse, groupRolesResponse));

        //    var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

        //    var permissions = await permissionsService.GetUserPermissionsForGroupAsync(userId, groupId, cancellationToken);

        //    Assert.IsTrue(!permissions.Any());
        //}

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task PermissionsService_GetUserPermissionsAsync_ThrowsIfUserIdEmpty()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cancellationToken = CancellationToken.None;

            var rolesDataProvider = new Mock<IRolesDataProvider>().Object;
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var permissionsService = new PermissionsService(rolesDataProvider, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var userId = Guid.Empty;

            _ = await permissionsService.GetUserPermissionsAsync(userId, cancellationToken);
        }


        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task PermissionsService_GetUserPermissionsAsync_ThrowsIfCancelled()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var rolesDataProvider = new Mock<IRolesDataProvider>().Object;
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            cts.Cancel();

            var permissionsService = new PermissionsService(rolesDataProvider, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var userId = Guid.NewGuid();

            _ = await permissionsService.GetUserPermissionsAsync(userId, cancellationToken);
        }
        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsAsync_ReturnsGuestRoleIfUserNotFound()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var userId = Guid.NewGuid();

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            List<string>? userRolesResponse = null;


            rolesDataProvider.Setup(x =>
                    x.GetUserRolesAsync(userId, cancellationToken))
                .ReturnsAsync(userRolesResponse);


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsAsync(userId, cancellationToken);
            Assert.IsTrue(permissions is null);
        }

        [TestMethod]
        public async Task PermissionsService_GetUserPermissionsAsync_ReturnsMultipleRolesAndPermissionsForUser()
        {
            var permissionsDataProviderLogger = new Moq.Mock<ILogger<PermissionsDataProvider>>().Object;
            var permissionsServiceLogger = new Moq.Mock<ILogger<PermissionsService>>().Object;
            var groupDataProvider = new Moq.Mock<IGroupDataProvider>().Object;

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var userId = Guid.NewGuid();

            var rolesDataProvider = new Mock<IRolesDataProvider>();
            IPermissionsDataProvider permissionsDataProvider = new PermissionsDataProvider(permissionsDataProviderLogger);

            var userRolesResponse = new List<string> { "Standard Members", "Admin" };


            rolesDataProvider.Setup(x =>
                    x.GetUserRolesAsync(userId, cancellationToken))
                .ReturnsAsync(userRolesResponse);


            var permissionsService = new PermissionsService(rolesDataProvider.Object, permissionsDataProvider, permissionsServiceLogger, groupDataProvider);

            var permissions = await permissionsService.GetUserPermissionsAsync(userId, cancellationToken);
            Assert.IsTrue(permissions.Any(x => x == "https://schema.collaborate.future.nhs.uk/admin/v1/view"));
        }
    }
}