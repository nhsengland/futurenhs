
namespace MvcForum.Web.Controllers
{
    using Core;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Core.Models.General;
    using MvcForum.Core.Models.Groups;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Web.ViewModels.Shared;
    using MvcForum.Web.ViewModels.Topic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels.Breadcrumb;
    using ViewModels.Group;
    using ViewModels.Mapping;
    using Constants = MvcForum.Core.Constants.Constants;

    [Authorize]
    public partial class GroupController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IGroupService _groupService;
        private readonly IFavouriteService _favouriteService;
        private readonly IPollService _pollAnswerService;
        private readonly IPostService _postService;
        private readonly IRoleService _roleService;
        private readonly ITopicService _topicService;
        private readonly IVoteService _voteService;
        private readonly IFeatureManager _featureManager;
        private readonly ILocalizationService _localizationService;
        private readonly IImageCommand _imageCommand;

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
            IMvcForumContext context, INotificationService notificationService, IFeatureManager featureManager,
            IImageCommand imageCommand)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _groupService = GroupService;
            _topicService = topicService;
            _pollAnswerService = pollService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
            _postService = postService;
            _roleService = roleService;
            _featureManager = featureManager;
            _localizationService = localizationService;
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
            _imageCommand = imageCommand ?? throw new ArgumentNullException(nameof(imageCommand));
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult MyGroupsList()
        {
            // TODO - OutputCache and add clear to post/topic/Group delete/create/edit

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListSummaryViewModel
            {
                AllPermissionSets =
                    ViewModelMapping.GetPermissionsForGroups(_groupService.GetAllMyGroupsInSummary(LoggedOnReadOnlyUser.Id),
                        _roleService, loggedOnUsersRole)
            };
            return PartialView("_MyGroups", catViewModel);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult DiscoverGroupsList()
        {
            // TODO - OutputCache and add clear to post/topic/Group delete/create/edit
            if (LoggedOnReadOnlyUser is null)
            {
                throw new ArgumentNullException(nameof(LoggedOnReadOnlyUser));
            }

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListSummaryViewModel
            {
                AllPermissionSets =
                    ViewModelMapping.GetPermissionsForGroups(_groupService.GetDiscoverGroupsInSummary(LoggedOnReadOnlyUser.Id),
                        _roleService, loggedOnUsersRole)
            };
            return PartialView("_DiscoverGroups", catViewModel);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult ListMainGroups()
        {
            // TODO - OutputCache and add clear to post/topic/Group delete/create/edit
            if (LoggedOnReadOnlyUser is null)
            {
                throw new ArgumentNullException(nameof(LoggedOnReadOnlyUser));
            }

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListSummaryViewModel
            {
                AllPermissionSets =
                    ViewModelMapping.GetPermissionsForGroups(_groupService.GetAllMainGroupsInSummary(LoggedOnReadOnlyUser.Id),
                        _roleService, loggedOnUsersRole)
            };
            return PartialView(catViewModel);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult ListSections()
        {
            // TODO - How can we cache this effectively??
            // Get all sections, and include all Groups

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Model for the sections
            var allSections = new List<SectionListViewModel>();

            // Get sections from the DB
            var dbSections = _groupService.GetAllSections();

            // Get all Groups grouped by section
            var groupedGroups = _groupService.GetAllMainGroupsInSummaryGroupedBySection(LoggedOnReadOnlyUser?.Id);

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

            MyGroupListViewModel MyGroupsModel;
            if (LoggedOnReadOnlyUser != null)
            {
                MyGroupsModel = new MyGroupListViewModel
                {
                    MyGroups = _groupService.GetAllForUser(LoggedOnReadOnlyUser.Id)
                };
            }
            else
            {
                MyGroupsModel = new MyGroupListViewModel
                {
                    MyGroups = new List<GroupUser>()
                };
            }

            //var catViewModel = new GroupListViewModel
            //{
            //    AllPermissionSets = ViewModelMapping.GetPermissionsForGroups(_groupService.GetAll(), _roleService,
            //            loggedOnUsersRole)
            //};
            return PartialView(MyGroupsModel);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult GetSubscribedGroups()
        {
            var viewModel = new List<ViewModels.Group.GroupViewModel>();

            var groups = LoggedOnReadOnlyUser.GroupNotifications.Select(x => x.Group);
            foreach (var group in groups)
            {
                var loggedOnUsersRole = GetGroupMembershipRole(group.Id);
                var permissionSet = RoleService.GetPermissions(group, loggedOnUsersRole);
                var topicCount = group.Topics.Count;
                var latestTopicInGroup =
                    group.Topics.OrderByDescending(x => x.LastPost.DateCreated).FirstOrDefault();
                var postCount = group.Topics.SelectMany(x => x.Posts).Count() - 1;
                var model = new ViewModels.Group.GroupViewModel
                {
                    Group = group,
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
        public virtual PartialViewResult GetGroupBreadcrumb(Group group)
        {
            var loggedOnUsersRole = GetGroupMembershipRole(group.Id);
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var viewModel = new BreadcrumbViewModel
            {
                Groups = _groupService.GetGroupParents(group, allowedGroups),
                Group = group
            };

            return PartialView("GetGroupBreadcrumb", viewModel);
        }

        [AsyncTimeout(30000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [ActionName("Join")]
        public virtual async Task<ActionResult> JoinAsync(string slug, int? p, CancellationToken cancellationToken)
        {
            await _groupService.JoinGroupAsync(slug, LoggedOnReadOnlyUser.Id, cancellationToken);
            return RedirectToAction("show", new { slug, p });
        }

        [HttpGet]
        [ActionName("Leave")]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [AsyncTimeout(30000)]
        public virtual async Task<ActionResult> LeaveAsync(string slug, int? p)
        {

            await _groupService.LeaveGroupAsync(slug, LoggedOnReadOnlyUser.Id);
            return RedirectToAction("show", new { slug, p });
        }

        public PartialViewResult GroupHeader(string slug, string tab = null)
        {
            var group = _groupService.GetBySlugWithSubGroups(slug, LoggedOnReadOnlyUser?.Id);

            var viewModel = new GroupHeaderViewModel()
            {
                Name = group.Group.Name,
                Description = group.Group.Description,
                Colour = group.Group.Colour,
                HeaderTabs = GetGroupTabsModel(slug, tab),
                Image = group.Group.Image,
                Id = group.Group.Id,
                ActionMenu = GetGroupActionMenu(group.Group)
            };

            return PartialView("_GroupHeader", viewModel);
        }

        public ActionMenuModel GetGroupActionMenu(Group group)
        {
            var model = new ActionMenuModel();

            if (LoggedOnReadOnlyUser == null) return model;

            var user = @group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id);
            var groupUserStatus = user.GetUserStatusForGroup();

            if (groupUserStatus != GroupUserStatus.Joined)
            {
                switch (groupUserStatus)
                {
                    case GroupUserStatus.NotJoined:
                        model.ActionButton = new Button
                        {
                            Name = LocalizationService.GetResourceString("Groups.Join"),
                            Url = Url.Action("Join", "Group", new { slug = @group.Slug }),
                            Active = true
                        };
                        break;
                    case GroupUserStatus.Pending:
                        model.ActionButton = new Button
                        {
                            Name = LocalizationService.GetResourceString("Pending Approval"),
                        };
                        break;
                    case GroupUserStatus.Locked:
                        model.ActionButton = new Button
                        {
                            Name = LocalizationService.GetResourceString("Locked Out"),
                        };
                        break;
                    case GroupUserStatus.Banned:
                        model.ActionButton = new Button
                        {
                            Name = LocalizationService.GetResourceString("Banned"),
                        };
                        break;

                }
            }
            else
            {
                model.ActionLinks = new List<ActionLink>
                {
                    new ActionLink {  Name = LocalizationService.GetResourceString("Groups.Leave") , Url = Url.Action("Leave", "Group", new { slug = @group.Slug }), Order = 1}
                };

                var loggedOnUsersRole = GetGroupMembershipRole(group.Id);
                if (loggedOnUsersRole.RoleName.ToLower() == "admin")
                {
                    model.ActionLinks.Add(new ActionLink
                    {
                        Name = "Add new member",
                        Url = Url.Action("AddMember", "GroupInvite", new { slug = @group.Slug }),
                        Order = 2
                    });
                    model.ActionLinks.Add(new ActionLink
                    {
                        Name = "Invite new user",
                        Url = Url.RouteUrl("GroupInviteUrls", new { slug = @group.Slug, groupId = @group.Id }),
                        Order = 3
                    });
                    model.ActionLinks.Add(new ActionLink
                    {
                        Name = "Edit group information",
                        Url = Url.RouteUrl("GroupEditUrls", new { slug = @group.Slug })
                    });
                }
            }

            return model;
        }

        // TODO Duplicated code from groups, we need to refactor all of this into one place.
        public TabViewModel GetGroupTabsModel(string slug, string tab)
        {
            var homeTab = new Tab
            {
                Name = "GroupTabs.Home", Order = 1,
                Url = $"{Url.RouteUrl("GroupUrls", new { slug, tab = UrlParameter.Optional })}"
            };

            var forumTab = new Tab
            {
                Name = "GroupTabs.Forum", Order = 2,
                Url = $"{Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupForumTab })}"
            };

            var filesTab = new Tab
            {
                Name = "GroupTabs.Files", Order = 3,
                Url = $"{Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupFilesTab })}"
            };

            var membersTab = new Tab
            {
                Name = "GroupTabs.Members", Order = 4,
                Url = $"{Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupMembersTab })}"
            };

            var aboutUsTab = new Tab
            {
                Name = "GroupTabs.AboutUs",
                Order = 5,
                Url = $"{Url.RouteUrl("GroupUrls", new { slug, tab = Constants.AboutUsTab })}"
            };



            switch (tab)
                {
                    case Constants.GroupFilesTab:
                        filesTab.Active = true;
                        break;
                    case Constants.GroupForumTab:
                        forumTab.Active = true;
                        break;
                    case Constants.GroupMembersTab:
                        membersTab.Active = true;
                        break;
                    case Constants.AboutUsTab:
                        aboutUsTab.Active = true;
                        break;
                default:
                        homeTab.Active = true;
                        break;
                }
            

            var tabsViewModel = new TabViewModel { Tabs = new List<Tab> { homeTab, forumTab, membersTab, filesTab, aboutUsTab } };

            return tabsViewModel;
        }

        [HttpGet]
        public PartialViewResult GetGroupHomeCards(string slug)
        {
            var viewModel = new GroupHomeCardsViewModel 
            { 
                ForumCard = new Tab { Url = Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupForumTab }), Name = "Join in the conversation" },
                MembersCard = new Tab { Url = Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupMembersTab }), Name = "Meet the members" },
                FilesCard = new Tab { Url = Url.RouteUrl("GroupUrls", new { slug, tab = Constants.GroupFilesTab }), Name = "View Files" },
            };

            return PartialView("_GroupHomeCards", viewModel);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult GetGroupForum(Guid groupId, int? p)
        {
            var group = _groupService.Get(groupId);
            var loggedOnUsersRole = GetGroupMembershipRole(groupId);

            // Allowed Groups for this user
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(group, loggedOnUsersRole);
            List<TopicViewModel> topicViewModels = new List<TopicViewModel>();
            PaginatedList<Topic> topics = new PaginatedList<Topic>(new List<Topic>(), 0, 0, 0);

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {


                if (loggedOnUsersRole.RoleName != MvcForum.Core.Constants.Constants.GuestRoleName || group.PublicGroup)
                {
                    topics = _topicService.GetPagedTopicsByGroup(pageIndex,
                        SettingsService.GetSettings().DiscussionsPerPage,
                        int.MaxValue, group.Id);

                    if (!allowedGroups.Contains(group))
                    {
                        allowedGroups.Add(group);
                    }

                    topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService,
                        loggedOnUsersRole, LoggedOnReadOnlyUser, allowedGroups, SettingsService.GetSettings(),
                        _postService, _notificationService, _pollAnswerService, _voteService, _favouriteService);
                }
            }

            var topicViewModel = new GroupTopicsViewModel
            {
                Topics = topicViewModels,
                PageIndex = pageIndex,
                TotalCount = topics.TotalCount,
                TotalPages = topics.TotalPages,
                PublicGroup = group.PublicGroup,
                GroupId =  group.Id

            };

            if (LoggedOnReadOnlyUser != null)
            {

                var userGroupStatus = group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id).GetUserStatusForGroup();
                topicViewModel.GroupUserStatus = userGroupStatus;
                topicViewModel.IsMember = userGroupStatus == GroupUserStatus.Joined;
                topicViewModel.LoggedInUserRole = loggedOnUsersRole;
            }

            return PartialView("_Forum", topicViewModel);
        }

        public virtual PartialViewResult LoadMoreTopics(Guid groupId, int? p)
        {
            var group = _groupService.Get(groupId);
            var loggedOnUsersRole = GetGroupMembershipRole(groupId);

            // Allowed Groups for this user
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(group, loggedOnUsersRole);
            var topicViewModels = new List<TopicViewModel>();

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {


                if (loggedOnUsersRole.RoleName != MvcForum.Core.Constants.Constants.GuestRoleName || group.PublicGroup)
                {
                    var topics = _topicService.GetPagedTopicsByGroup(pageIndex,
                        SettingsService.GetSettings().DiscussionsPerPage,
                        int.MaxValue, @group.Id);

                    if (!allowedGroups.Contains(group))
                    {
                        allowedGroups.Add(group);
                    }

                    topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService,
                        loggedOnUsersRole, LoggedOnReadOnlyUser, allowedGroups, SettingsService.GetSettings(),
                        _postService, _notificationService, _pollAnswerService, _voteService, _favouriteService);
                }
            }

            return PartialView("_Topics", topicViewModels);
        }

        [HttpGet]
        [ActionName("Show")]
        public virtual ActionResult Show(string slug, int? p, string tab = null, Guid? folder = null, bool? hasError = null)
        {
            // Get the Group
            var group = _groupService.GetBySlugWithSubGroups(slug, LoggedOnReadOnlyUser?.Id);
            var loggedOnUsersRole = GetGroupMembershipRole(group.Group.Id);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(group.Group, loggedOnUsersRole);

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                var groupUser = group.Group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id);
                // Create the main view model for the Group
                var viewModel = new ViewModels.Group.GroupViewModel
                {
                    Permissions = permissions,
                    Group = group.Group,
                    PageIndex = pageIndex,
                    User = LoggedOnReadOnlyUser,
                    GroupUserStatus = groupUser.GetUserStatusForGroup(),
                    GroupUserRole = GetGroupMembershipRole(group.Group.Id),
                    IsSubscribed = User.Identity.IsAuthenticated && _notificationService.GetGroupNotificationsByUserAndGroup(LoggedOnReadOnlyUser, group.Group).Any(),
                    Tab = tab,
                    Folder = folder,
                    HasError = hasError
                };

                viewModel.IsMember = viewModel.GroupUserStatus == GroupUserStatus.Joined; 

                if (loggedOnUsersRole.RoleName != Constants.GuestRoleName)

                    // If there are subGroups then add then with their permissions
                    if (group.SubGroups.Any())
                    {
                        var subCatViewModel = new GroupListViewModel
                        {
                            AllPermissionSets = new Dictionary<Group, PermissionSet>()
                        };
                        foreach (var subGroup in group.SubGroups)
                        {
                            var subGroupRole = GetGroupMembershipRole(subGroup.Id);
                            var permissionSet = RoleService.GetPermissions(subGroup, subGroupRole);
                            subCatViewModel.AllPermissionSets.Add(subGroup, permissionSet);
                        }
                        viewModel.SubGroups = subCatViewModel;
                    }


                if (tab == Constants.GroupMembersTab)
                {
                    ViewBag.HideSideBar = true;
                }

                if (tab == Constants.GroupFilesTab)
                {
                    ViewBag.HideSideBar = true;
                }
                if (tab == Constants.AboutUsTab)
                {
                    ViewBag.HideSideBar = true;
                }
                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }


        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult AboutUs(string slug)
        {  // Get the Group
            var group =  _groupService.Get(slug);
            return PartialView("_AboutUs", group);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult GetGroupFiles(Guid groupId, int? p)
        {
            return PartialView("_Files", null);
        }

        [HttpGet]
        [ChildActionOnly]
        public virtual PartialViewResult GetGroupMembers(Guid groupId, int? p)
        {
            // Get the Group
            var group = _groupService.Get(groupId);

            var loggedOnUsersRole = GetGroupMembershipRole(group.Id);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(group, loggedOnUsersRole);

            var groupUsers = group.GroupUsers.Select(x => new GroupUserViewModel { GroupUser = x, GroupUserStatus = x.GetUserStatusForGroup() });

            // Create the main view model for the Group
            var viewModel = new GroupMembersViewModel {

                PageIndex = pageIndex,
                TotalCount = group.GroupUsers.Count,
                TotalPages = (int)Math.Ceiling(group.GroupUsers.Count / (double)SettingsService.GetSettings().DiscussionsPerPage),
                GroupUsers = groupUsers.Where(x => x.GroupUserStatus != GroupUserStatus.Pending && 
                                                   x.GroupUserStatus != GroupUserStatus.Rejected &&
                                                   x.GroupUser.Role.RoleName != Constants.AdminRoleName).ToList()
                                                   .OrderBy(x => x.GroupUser.User.Surname),
                GroupUsersPending = groupUsers.Where(x => x.GroupUserStatus == GroupUserStatus.Pending).ToList(),
                GroupAdmins = groupUsers.Where(x => x.GroupUser.Role.RoleName == Constants.AdminRoleName).ToList().OrderBy(x => x.GroupUser.User.Surname),
                PublicGroup = group.PublicGroup
            };


            if (LoggedOnReadOnlyUser != null)
            {

                var user = group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id);
                viewModel.GroupUserStatus = user.GetUserStatusForGroup();
                viewModel.LoggedInUserRole = loggedOnUsersRole;
            }

            return PartialView("_ManageUsers", viewModel);
        }

        [HttpGet]
        public virtual ActionResult ManageUser(Guid groupUserId)
        {
            // Get the Group
            var groupUser = _groupService.GetGroupUser(groupUserId);
            var roles = _roleService.AllRoles().Where(x => x.RoleName != Constants.GuestRoleName);
            var selectList = new List<SelectListItem>(roles.Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }));

            var viewModel = new GroupUserViewModel
            {
                GroupUser = groupUser,
                GroupUserStatus = groupUser.GetUserStatusForGroup(),
                RoleSelectList = selectList,
                MemberRole = GetGroupMembershipRole(groupUser.Group.Id)

            };

            return View(viewModel);
        }

        [HttpGet]
        public virtual ActionResult ApproveUser(Guid groupUserId, string slug)
        {
            _groupService.ApproveJoinGroup(groupUserId, LoggedOnReadOnlyUser.Id);
            return RedirectToRoute("GroupUrls", new { slug, tab = Constants.GroupMembersTab });
        }

        [HttpGet]
        public virtual ActionResult RejectUser(Guid groupUserId, string slug)
        {
            _groupService.RejectJoinGroup(groupUserId, LoggedOnReadOnlyUser.Id);
            return RedirectToRoute("GroupUrls", new { slug, tab = Constants.GroupMembersTab });
        }

        [HttpPost]
        [ActionName("ManageUser")]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [AsyncTimeout(30000)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ManageUserAsync(GroupUserViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the Group
            var groupUser = await _groupService.UpdateGroupUserAsync(model.GroupUser, cancellationToken);
            var roles = _roleService.AllRoles();
            var selectList = new List<SelectListItem>(roles.Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }));

            var viewModel = new GroupUserViewModel
            {
                GroupUser = groupUser,
                GroupUserStatus = groupUser.GetUserStatusForGroup(),
                RoleSelectList = selectList,
                MemberRole = GetGroupMembershipRole(groupUser.Group.Id)

            };

            return View(viewModel);
        }

        [HttpGet]
        private MembershipRole GetGroupMembershipRole(Guid groupId)
        {

            var role = _groupService.GetGroupRole(groupId, LoggedOnReadOnlyUser?.Id);
            if (LoggedOnReadOnlyUser == null)
                return role;

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            return loggedOnUsersRole.RoleName == MvcForum.Core.Constants.Constants.AdminRoleName ? loggedOnUsersRole : role;
        }

        [HttpGet]
        [AsyncTimeout(30000)]
        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string slug, string tab, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            if (string.IsNullOrWhiteSpace(tab)) throw new ArgumentNullException(nameof(tab));

            if (!_groupService.UserIsAdmin(slug, LoggedOnReadOnlyUser.Id)) return new HttpUnauthorizedResult();

            var group = await _groupService.GetAsync(slug, cancellationToken);

            if (group is null) return HttpNotFound();

            var model = new GroupWriteViewModel
            {
                Id = group.Id,
                Description = group.Description,
                Image = group.Image,
                Introduction = group.Introduction,
                AboutUs = group.AboutUs,
                Name = group.Name,
                PublicGroup = group.PublicGroup,
                Slug = group.Slug,
            };

            return View(model);
        }

        [HttpPost]
        [AsyncTimeout(30000)]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(GroupWriteViewModel model, string slug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            if (this.ModelState.IsValid)
            {
                if (!_groupService.UserIsAdmin(slug, LoggedOnReadOnlyUser.Id)) return new HttpUnauthorizedResult();

                if (model.Files != null)
                {
                    var uploadResult = _groupService.UploadGroupImage(model.Files, model.Id);
                    if (uploadResult.UploadSuccessful)
                    {
                        model.Image = uploadResult.UploadedFileName;
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Files), "Failed to upload file");
                    }
                }

                if (ModelState.IsValid && await _groupService.UpdateAsync(model, slug, cancellationToken))
                {
                    return RedirectToAction("Show", "Group", new { slug, tab = String.Empty });
                }
            }

            return View(model);
        }

    }
}
