namespace MvcForum.Core.Tests.Services
{
    using Moq;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Groups;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using MvcForum.Core.Services;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    [TestFixture]
    public class GroupServiceTests
    {
        private IGroupService _groupService;

        private readonly Mock<IRoleService> _roleService;
        private readonly Mock<INotificationService> _notificationService;
        private readonly Mock<IGroupPermissionForRoleService> _groupPermissionForRoleService;
        private readonly Mock<ICacheService> _cacheService;
        private readonly Mock<ILocalizationService> _localizationService;
        private readonly Mock<IGroupRepository> _groupRepository;
        private readonly Mock<IMvcForumContext> _context;
        private readonly Mock<IGroupCommand> _groupCommand;
        private readonly Mock<IImageService> _imageService;
        private readonly Mock<IImageCommand> _imageCommand;
        private readonly Mock<IImageRepository> _imageRepository;

        private Mock<HttpPostedFileBase> _postedFile;

        private GroupWriteViewModel _groupWriteViewModel;
        private GroupViewModel _groupViewModel;

        private const string GROUP_NAME = "a group";
        private const string GROUP_INTRODUCTION = "a group introduction";
        private const string GROUP_DESCRIPTION = "a group description";
        private const string GROUP_IMAGE = "image-name";

        private const string SLUG_NOT_VALID = "invalid slug";

        private const string SLUG_VALID = "valid slug";

        private const string SLUG_PARAMETER_NAME = "slug";
        private const string GROUP_SLUG_PARAMETER_NAME = "groupSlug";
        private const string ID_PARAMETER_NAME = "id";
        private const string GROUP_ID_PARAMETER_NAME = "groupId";
        private const string USER_ID_PARAMETER_NAME = "userId";
        private const string MODEL_PARAMETER_NAME = "model";
        private const string FILE_PARAMETER_NAME = "file";

        public GroupServiceTests()
        {
            _roleService = new Mock<IRoleService>();
            _notificationService = new Mock<INotificationService>();
            _groupPermissionForRoleService = new Mock<IGroupPermissionForRoleService>();
            _cacheService = new Mock<ICacheService>();
            _localizationService = new Mock<ILocalizationService>();
            _groupRepository = new Mock<IGroupRepository>();
            _context = new Mock<IMvcForumContext>();
            _groupCommand = new Mock<IGroupCommand>();
            _imageService = new Mock<IImageService>();
            _imageCommand = new Mock<IImageCommand>();
            _imageRepository = new Mock<IImageRepository>();

        }

        [SetUp]
        public void SetUp()
        {
            _groupWriteViewModel = new GroupWriteViewModel
            {
                Name = GROUP_NAME,
                Description = GROUP_DESCRIPTION,
                Introduction = GROUP_INTRODUCTION,
                Image = GROUP_IMAGE,
                PublicGroup = true,
            };

            _groupViewModel = new GroupViewModel
            {
                Name = GROUP_NAME,
                Description = GROUP_DESCRIPTION,
                Introduction = GROUP_INTRODUCTION,
                Image = GROUP_IMAGE,
                PublicGroup = true,
            };

            _groupService = new GroupService(
                _context.Object,
                _roleService.Object,
                _notificationService.Object,
                _groupPermissionForRoleService.Object,
                _cacheService.Object,
                _groupRepository.Object,
                _localizationService.Object,
                _groupCommand.Object,
                _imageService.Object,
                _imageCommand.Object,
                _imageRepository.Object
            );

            _postedFile = new Mock<HttpPostedFileBase>();
        }

        // GetAsync - slug

        [Test]
        public void GetAsync_NullSlug_ThrowsException()
        {
            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupService.GetAsync(null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public async Task GetAsync_NoGroupFoundForSlug_ReturnsNull()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<GroupViewModel>(null));

            var result = await _groupService.GetAsync(SLUG_NOT_VALID, CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAsync_GroupFoundForSlug_ReturnsGroup()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(_groupViewModel));

            var result = await _groupService.GetAsync(SLUG_VALID, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(GROUP_NAME, result.Name);
            Assert.AreEqual(GROUP_DESCRIPTION, result.Description);
            Assert.AreEqual(GROUP_INTRODUCTION, result.Introduction);
            Assert.AreEqual(GROUP_IMAGE, result.Image);
            Assert.IsTrue(result.PublicGroup);
        }

        // GetAsync - id

        [Test]
        public void GetAsync_EmptyId_ThrowsException()
        {
            var result = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _groupService.GetAsync(Guid.Empty, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(result);
            Assert.AreEqual(ID_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public async Task GetAsync_NoGroupFoundForId_ReturnsNull()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<GroupViewModel>(null));

            var result = await _groupService.GetAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAsync_GroupFoundForId_ReturnsGroup()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(_groupViewModel));

            var result = await _groupService.GetAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(GROUP_NAME, result.Name);
            Assert.AreEqual(GROUP_DESCRIPTION, result.Description);
            Assert.AreEqual(GROUP_INTRODUCTION, result.Introduction);
            Assert.AreEqual(GROUP_IMAGE, result.Image);
            Assert.IsTrue(result.PublicGroup);
        }

        // GetBySlugAsync - slug

        [Test]
        public void GetBySlugAsync_NullSlug_ThrowsException()
        {
            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupService.GetBySlugAsync(null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public async Task GetBySlugAsync_NoGroupFoundForSlug_ReturnsNull()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<GroupViewModel>(null));

            var result = await _groupService.GetBySlugAsync(SLUG_NOT_VALID, CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetBySlugAsync_GroupFoundForSlug_ReturnsGroup()
        {
            _groupRepository.Setup(x => x.GetGroupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(_groupViewModel));

            var result = await _groupService.GetBySlugAsync(SLUG_VALID, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(GROUP_NAME, result.Name);
            Assert.AreEqual(GROUP_DESCRIPTION, result.Description);
            Assert.AreEqual(GROUP_INTRODUCTION, result.Introduction);
            Assert.AreEqual(GROUP_IMAGE, result.Image);
            Assert.IsTrue(result.PublicGroup);
        }

        // UserIsAdmin 

        [Test]
        public void UserIsAdmin_SlugNullUserIdEmpty_ThrowsSlugException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UserIsAdmin(null, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(GROUP_SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserIsAdmin_SlugHasValueUserIdEmpty_ThrowsIdException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(delegate { _groupService.UserIsAdmin(SLUG_VALID, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(result);
            Assert.AreEqual(USER_ID_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserIsAdmin_SlugNullUserIdHasValue_ThrowsSlugException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UserIsAdmin(null, Guid.NewGuid()); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(GROUP_SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserIsAdmin_NotAdmin_ReturnsFalse()
        {
            _groupRepository.Setup(x => x.UserIsAdmin(It.IsAny<string>(), It.IsAny<Guid>())).Returns(false);

            var result = _groupService.UserIsAdmin(SLUG_VALID, Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public void UserIsAdmin_IsAdmin_ReturnsTrue()
        {
            _groupRepository.Setup(x => x.UserIsAdmin(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);

            var result = _groupService.UserIsAdmin(SLUG_VALID, Guid.NewGuid());

            Assert.IsTrue(result);
        }

        // UserHasGroupAccess

        [Test]
        public void UserHasGroupAccess_SlugNullUserIdEmpty_ThrowsSlugException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UserHasGroupAccess(null, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(GROUP_SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserHasGroupAccess_SlugHasValueUserIdEmpty_ThrowsIdException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(delegate { _groupService.UserHasGroupAccess(SLUG_VALID, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(result);
            Assert.AreEqual(USER_ID_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserHasGroupAccess_SlugNullUserIdHasValue_ThrowsSlugException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UserHasGroupAccess(null, Guid.NewGuid()); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(GROUP_SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UserHasGroupAccess_NoAccess_ReturnsFalse()
        {
            _groupRepository.Setup(x => x.UserHasGroupAccess(It.IsAny<string>(), It.IsAny<Guid>())).Returns(false);

            var result = _groupService.UserHasGroupAccess(SLUG_VALID, Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public void UserHasGroupAccess_HasAccess_ReturnsTrue()
        {
            _groupRepository.Setup(x => x.UserHasGroupAccess(It.IsAny<string>(), It.IsAny<Guid>())).Returns(true);

            var result = _groupService.UserHasGroupAccess(SLUG_VALID, Guid.NewGuid());

            Assert.IsTrue(result);
        }

        // UpdateAsync

        [Test]
        public void UpdateAsync_ModelNull_ThrowsException()
        {
            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupService.UpdateAsync(null, SLUG_PARAMETER_NAME, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(MODEL_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UpdateAsync_SlugNull_ThrowsException()
        {
            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await _groupService.UpdateAsync(_groupWriteViewModel, null, CancellationToken.None));

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(SLUG_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public async Task UpdateAsync_ValidUpdate_ReturnsTrue()
        {
            _groupCommand.Setup(m => m.UpdateAsync(It.IsAny<GroupWriteViewModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _groupService.UpdateAsync(_groupWriteViewModel, SLUG_PARAMETER_NAME);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateAsync_InvalidUpdate_ReturnsFalse()
        {
            _groupCommand.Setup(m => m.UpdateAsync(It.IsAny<GroupWriteViewModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _groupService.UpdateAsync(_groupWriteViewModel, SLUG_PARAMETER_NAME);

            Assert.IsFalse(result);
        }

        // UploadGroupImage

        [Test]
        public void UploadGroupImage_PostedFileNullGroupIdEmpty_ThrowsNullException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UploadGroupImage(null, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(FILE_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UploadGroupImage_PostedFileNullGroupIdHasValue_ThrowsNullException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _groupService.UploadGroupImage(null, Guid.NewGuid()); });

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(FILE_PARAMETER_NAME, result.ParamName);
        }

        [Test]
        public void UploadGroupImage_HasPostedFileGroupIdEmpty_ThrowsOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(delegate { _groupService.UploadGroupImage(_postedFile.Object, Guid.Empty); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(result);
            Assert.AreEqual(GROUP_ID_PARAMETER_NAME, result.ParamName);
        }

        // Note no further tests added for UploadGroupImage due to static methods being used and the need to refactor which could be a big task
    }
}
