namespace MvcForum.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using ViewModels;
    using ViewModels.Moderate;

    [Authorize]
    public partial class ModerateController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly IActivityService _activityService;

        public ModerateController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IPostService postService, ITopicService topicService, IGroupService GroupService,
            ICacheService cacheService, IMvcForumContext context, IActivityService activityService)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _postService = postService;
            _topicService = topicService;
            _groupService = GroupService;
            _activityService = activityService;
        }

        public virtual ActionResult Index()
        {
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Show both pending topics and also pending posts
            // Use ajax for paging too
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var viewModel = new ModerateViewModel
            {
                Posts = _postService.GetPendingPosts(allowedGroups, loggedOnUsersRole),
                Topics = _topicService.GetPendingTopics(allowedGroups, loggedOnUsersRole)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ActionName("ModerateTopic")]
        [AsyncTimeout(30000)]
        public virtual async Task<ActionResult> ModerateTopicAsync(ModerateActionViewModel viewModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                User.GetMembershipUser(MembershipService);
                var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

                var topic = _topicService.Get(viewModel.TopicId);
                var permissions = RoleService.GetPermissions(topic.Group, loggedOnUsersRole);

                // Is this user allowed to moderate - We use EditPosts for now until we change the permissions system
                if (!permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
                {
                    return Content(LocalizationService.GetResourceString("Errors.NoPermission"));
                }

                if (viewModel.IsApproved)
                {
                    topic.Pending = false;
                    _activityService.TopicCreated(topic);
                }
                else
                {
                    var topicResult = await _topicService.Delete(topic);
                    if (!topicResult.Successful)
                    {
                        return Content(topicResult.ProcessLog.FirstOrDefault());
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                return Content(ex.Message);
            }


            return Content("allgood");
        }

        [HttpPost]
        public virtual ActionResult ModeratePost(ModerateActionViewModel viewModel)
        {
            try
            {
                User.GetMembershipUser(MembershipService);
                var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

                var post = _postService.Get(viewModel.PostId);
                var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);
                if (!permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
                {
                    return Content(LocalizationService.GetResourceString("Errors.NoPermission"));
                }

                if (viewModel.IsApproved)
                {
                    post.Pending = false;
                    _activityService.PostCreated(post);
                }
                else
                {
                    _postService.Delete(post, false);
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                return Content(ex.Message);
            }


            return Content("allgood");
        }
    }
}