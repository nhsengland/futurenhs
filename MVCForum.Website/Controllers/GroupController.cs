namespace MvcForum.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Application.CustomActionResults;
    using Core;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Core.Models.General;
    using ViewModels.Breadcrumb;
    using ViewModels.Group;
    using ViewModels.Mapping;

    public partial class GroupController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IGroupService _GroupService;
        private readonly IFavouriteService _favouriteService;
        private readonly IPollService _pollAnswerService;
        private readonly IPostService _postService;
        private readonly IRoleService _roleService;
        private readonly ITopicService _topicService;
        private readonly IVoteService _voteService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="loggingService"> </param>
        /// <param name="membershipService"></param>
        /// <param name="localizationService"></param>
        /// <param name="roleService"></param>
        /// <param name="GroupService"></param>
        /// <param name="settingsService"> </param>
        /// <param name="topicService"> </param>
        /// <param name="cacheService"></param>
        /// <param name="postService"></param>
        /// <param name="pollService"></param>
        /// <param name="voteService"></param>
        /// <param name="favouriteService"></param>
        /// <param name="context"></param>
        /// <param name="notificationService"></param>
        public GroupController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, IGroupService GroupService,
            ISettingsService settingsService, ITopicService topicService,
            ICacheService cacheService,
            IPostService postService,
            IPollService pollService, IVoteService voteService, IFavouriteService favouriteService,
            IMvcForumContext context, INotificationService notificationService)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _GroupService = GroupService;
            _topicService = topicService;
            _pollAnswerService = pollService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
            _postService = postService;
            _roleService = roleService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public virtual PartialViewResult ListMainGroups()
        {
            // TODO - OutputCache and add clear to post/topic/Group delete/create/edit

            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListSummaryViewModel
            {
                AllPermissionSets =
                    ViewModelMapping.GetPermissionsForGroups(_GroupService.GetAllMainGroupsInSummary(),
                        _roleService, loggedOnUsersRole)
            };
            return PartialView(catViewModel);
        }


        [ChildActionOnly]
        public virtual PartialViewResult ListSections()
        {
            // TODO - How can we cache this effectively??
            // Get all sections, and include all Groups

            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Model for the sections
            var allSections = new List<SectionListViewModel>();

            // Get sections from the DB
            var dbSections = _GroupService.GetAllSections();

            // Get all Groups grouped by section
            var groupedGroups = _GroupService.GetAllMainGroupsInSummaryGroupedBySection();

            // Loop sections
            foreach (var dbSection in dbSections)
            {
                var GroupsInSection = groupedGroups[dbSection.Id];
                var allPermissionSets = ViewModelMapping.GetPermissionsForGroups(GroupsInSection, _roleService, loggedOnUsersRole, true);

                allSections.Add(new SectionListViewModel
                {
                    Section = dbSection,
                    AllPermissionSets = allPermissionSets
                });

            }

            return PartialView(allSections);
        }

        [ChildActionOnly]
        public virtual PartialViewResult ListGroupSideMenu()
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListViewModel
            {
                AllPermissionSets = ViewModelMapping.GetPermissionsForGroups(_GroupService.GetAll(), _roleService,
                        loggedOnUsersRole)
            };
            return PartialView(catViewModel);
        }

        [Authorize]
        [ChildActionOnly]
        public virtual PartialViewResult GetSubscribedGroups()
        {
            var viewModel = new List<GroupViewModel>();

            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var Groups = loggedOnReadOnlyUser.GroupNotifications.Select(x => x.Group);
            foreach (var Group in Groups)
            {
                var permissionSet = RoleService.GetPermissions(Group, loggedOnUsersRole);
                var topicCount = Group.Topics.Count;
                var latestTopicInGroup =
                    Group.Topics.OrderByDescending(x => x.LastPost.DateCreated).FirstOrDefault();
                var postCount = Group.Topics.SelectMany(x => x.Posts).Count() - 1;
                var model = new GroupViewModel
                {
                    Group = Group,
                    LatestTopic = latestTopicInGroup,
                    Permissions = permissionSet,
                    PostCount = postCount,
                    TopicCount = topicCount,
                    ShowUnSubscribedLink = true
                };
                viewModel.Add(model);
            }


            return PartialView(viewModel);
        }


        [ChildActionOnly]
        public virtual PartialViewResult GetGroupBreadcrumb(Group Group)
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var allowedGroups = _GroupService.GetAllowedGroups(loggedOnUsersRole);
            var viewModel = new BreadcrumbViewModel
            {
                Groups = _GroupService.GetGroupParents(Group, allowedGroups),
                Group = Group
            };
            return PartialView("GetGroupBreadcrumb", viewModel);
        }

        public virtual async Task<ActionResult> Show(string slug, int? p)
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Get the Group
            var Group = _GroupService.GetBySlugWithSubGroups(slug);

            // Allowed Groups for this user
            var allowedGroups = _GroupService.GetAllowedGroups(loggedOnUsersRole);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(Group.Group, loggedOnUsersRole);

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                var topics = await _topicService.GetPagedTopicsByGroup(pageIndex,
                    SettingsService.GetSettings().TopicsPerPage,
                    int.MaxValue, Group.Group.Id);

                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService,
                    loggedOnUsersRole, loggedOnReadOnlyUser, allowedGroups, SettingsService.GetSettings(),
                    _postService, _notificationService, _pollAnswerService, _voteService, _favouriteService);

                // Create the main view model for the Group
                var viewModel = new GroupViewModel
                {
                    Permissions = permissions,
                    Topics = topicViewModels,
                    Group = Group.Group,
                    PageIndex = pageIndex,
                    TotalCount = topics.TotalCount,
                    TotalPages = topics.TotalPages,
                    User = loggedOnReadOnlyUser,
                    IsSubscribed = User.Identity.IsAuthenticated && _notificationService
                                       .GetGroupNotificationsByUserAndGroup(loggedOnReadOnlyUser, Group.Group).Any()
                };

                // If there are subGroups then add then with their permissions
                if (Group.SubGroups.Any())
                {
                    var subCatViewModel = new GroupListViewModel
                    {
                        AllPermissionSets = new Dictionary<Group, PermissionSet>()
                    };
                    foreach (var subGroup in Group.SubGroups)
                    {
                        var permissionSet = RoleService.GetPermissions(subGroup, loggedOnUsersRole);
                        subCatViewModel.AllPermissionSets.Add(subGroup, permissionSet);
                    }
                    viewModel.SubGroups = subCatViewModel;
                }

                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public virtual ActionResult GroupRss(string slug)
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // get an rss lit ready
            var rssTopics = new List<RssItem>();

            // Get the Group
            var Group = _GroupService.Get(slug);

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(Group, loggedOnUsersRole);

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                var topics = _topicService.GetRssTopicsByGroup(50, Group.Id);

                rssTopics.AddRange(topics.Select(x =>
                    {
                        var firstOrDefault =
                            x.Posts.FirstOrDefault(s => s.IsTopicStarter);
                        return firstOrDefault != null
                            ? new RssItem
                            {
                                Description = firstOrDefault.PostContent,
                                Link = x.NiceUrl,
                                Title = x.Name,
                                PublishedDate = x.CreateDate
                            }
                            : null;
                    }
                ));

                return new RssResult(rssTopics,
                    string.Format(LocalizationService.GetResourceString("Rss.Group.Title"), Group.Name),
                    string.Format(LocalizationService.GetResourceString("Rss.Group.Description"),
                        Group.Name));
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NothingToDisplay"));
        }
    }
}