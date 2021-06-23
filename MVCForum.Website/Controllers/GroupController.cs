using System;
using CommonServiceLocator;
using Microsoft.Ajax.Utilities;
using MvcForum.Web.ViewModels.Shared;
using MvcForum.Web.ViewModels.Topic;
using Constants = MvcForum.Core.Constants.Constants;

namespace MvcForum.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Application.CustomActionResults;
    using Core;
    using Core.Constants.UI;
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
        private readonly IGroupService _groupService;
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
            _groupService = GroupService;
            _topicService = topicService;
            _pollAnswerService = pollService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
            _postService = postService;
            _roleService = roleService;
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public virtual PartialViewResult ListMainGroups()
        {
            // TODO - OutputCache and add clear to post/topic/Group delete/create/edit

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var catViewModel = new GroupListSummaryViewModel
            {
                AllPermissionSets =
                    ViewModelMapping.GetPermissionsForGroups(_groupService.GetAllMainGroupsInSummary(LoggedOnReadOnlyUser?.Id),
                        _roleService, loggedOnUsersRole)
            };
            return PartialView(catViewModel);
        }


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
                    MyGroups = _groupService.GetAllForUser(LoggedOnReadOnlyUser?.Id)
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

        [Authorize]
        [ChildActionOnly]
        public virtual PartialViewResult GetSubscribedGroups()
        {
            var viewModel = new List<GroupViewModel>();

            var groups = LoggedOnReadOnlyUser.GroupNotifications.Select(x => x.Group);
            foreach (var group in groups)
            {
                var loggedOnUsersRole = GetGroupMembershipRole(group.Id);
                var permissionSet = RoleService.GetPermissions(group, loggedOnUsersRole);
                var topicCount = group.Topics.Count;
                var latestTopicInGroup =
                    group.Topics.OrderByDescending(x => x.LastPost.DateCreated).FirstOrDefault();
                var postCount = group.Topics.SelectMany(x => x.Posts).Count() - 1;
                var model = new GroupViewModel
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


        private GroupUserStatus GetUserStatusForGroup(GroupUser user)
        {
            if (user == null)
                return GroupUserStatus.NotJoined;
            if (user.Approved && !user.Banned && !user.Locked)
                return GroupUserStatus.Joined;
            if (!user.Approved && !user.Banned && !user.Locked && !user.Rejected)
                return GroupUserStatus.Pending;
            if (user.Approved && user.Banned && !user.Locked)
                return GroupUserStatus.Banned;
            if (user.Approved && !user.Banned && user.Locked)
                return GroupUserStatus.Locked;

            return user.Rejected ? GroupUserStatus.Rejected : GroupUserStatus.NotJoined;
        }

        public virtual async Task<ActionResult> Join(string slug, int? p)
        {
            _groupService.JoinGroup(slug, LoggedOnReadOnlyUser.Id);
            return RedirectToAction("show", new { slug = slug, p = p });
        }

        public virtual async Task<ActionResult> Leave(string slug, int? p)
        {

            _groupService.LeaveGroup(slug, LoggedOnReadOnlyUser.Id);
            return RedirectToAction("show", new { slug = slug, p = p });
        }

        /// <summary>
        /// Method to get the group tabs model.
        /// </summary>
        /// <param name="slug">The slug for the current group.</param>
        /// <returns>View model for the group tabs <see cref="TabViewModel"/>.</returns>
        public TabViewModel GetGroupTabsModel(string slug)
        {
            var forumTab = new Tab { Name = "GroupTabs.Forum", Order = 2, Icon = Icons.Forum };
            forumTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupForumTab })}";

            var membersTab = new Tab { Name = "GroupTabs.Members", Order = 3, Icon = Icons.Members };
            membersTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupMembersTab })}";

            var homeTab = new Tab { Name = "GroupTabs.Home", Order = 1, Icon = Icons.Home };
            homeTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = UrlParameter.Optional })}";

            var tabsViewModel = new TabViewModel { Tabs = new List<Tab> { homeTab, forumTab, membersTab } };

            return tabsViewModel;
        }

        public PartialViewResult GetGroupHomeCards(string slug)
        {
            var viewModel = new GroupHomeCardsViewModel 
            { 
                ForumCard = new Tab { Url = Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupForumTab }), Name = "Join in the conversation" },
                MembersCard = new Tab { Url = Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupMembersTab }), Name = "Meet the members" }
            };

            return PartialView("_GroupHomeCards", viewModel);

        }

        [ChildActionOnly]
        public virtual PartialViewResult GetGroupTabs(string activeTab, string slug)
        {

            var activeTabFound = false;
            var forumTab = new Tab { Name = "GroupTabs.Forum", Order = 2 ,Icon = Icons.Forum};
            if (activeTab == Constants.GroupForumTab)
            {
                forumTab.Active = true;
                activeTabFound = true;
            }

            forumTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupForumTab })}";

            var membersTab = new Tab { Name = "GroupTabs.Members", Order = 3, Icon = Icons.Members };
            if (activeTab == Constants.GroupMembersTab)
            {
                membersTab.Active = true;
                activeTabFound = true;
            }

            membersTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = Constants.GroupMembersTab })}";


            var homeTab = new Tab { Name = "GroupTabs.Home", Order = 1, Icon = Icons.Home };
            if (!activeTabFound)
            {
                homeTab.Active = true;
            }

            homeTab.Url = $"{Url.RouteUrl("GroupUrls", new { slug = slug, tab = UrlParameter.Optional })}";

            var tabsViewModel = new TabViewModel { Tabs = new List<Tab> { homeTab, forumTab, membersTab } };

            return PartialView("_TabsMenu", tabsViewModel);
        }

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
                        SettingsService.GetSettings().TopicsPerPage,
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

                var user = group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id);
                topicViewModel.GroupUserStatus = GetUserStatusForGroup(user);
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
                        SettingsService.GetSettings().TopicsPerPage,
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

        public virtual async Task<ActionResult> Show(string slug, int? p, string tab = null)
        {

            // Get the Group
            var group = _groupService.GetBySlugWithSubGroups(slug, LoggedOnReadOnlyUser?.Id);
            var loggedOnUsersRole = GetGroupMembershipRole(group.Group.Id);

            // Allowed Groups for this user
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            // Set the page index
            var pageIndex = p ?? 1;

            // check the user has permission to this Group
            var permissions = RoleService.GetPermissions(group.Group, loggedOnUsersRole);

            if (!permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                // Create the main view model for the Group
                var viewModel = new GroupViewModel
                {
                    Permissions = permissions,
                    Group = group.Group,
                    PageIndex = pageIndex,
                    User = LoggedOnReadOnlyUser,
                    GroupUserRole = GetGroupMembershipRole(group.Group.Id),
                    IsSubscribed = User.Identity.IsAuthenticated && _notificationService
                                       .GetGroupNotificationsByUserAndGroup(LoggedOnReadOnlyUser, group.Group).Any(),
                    Tab = tab
                };

                if (loggedOnUsersRole.RoleName != MvcForum.Core.Constants.Constants.GuestRoleName)

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


                if (LoggedOnReadOnlyUser != null)
                {

                    var user = viewModel.Group.GroupUsers.FirstOrDefault(x => x.User.Id == LoggedOnReadOnlyUser.Id);
                    viewModel.GroupUserStatus = GetUserStatusForGroup(user);
                }

                var pageHeader = new PageViewModel();
                pageHeader.Name = group.Group.Name;
                pageHeader.Description = group.Group.Description;
                pageHeader.Colour = group.Group.Colour;
                pageHeader.HeaderTabs = GetGroupTabsModel(slug);
                pageHeader.Image = group.Group.Image;
                pageHeader.Id = group.Group.Id;

                ViewBag.PageHeader =  pageHeader;
                if (tab == Constants.GroupMembersTab)
                {
                    ViewBag.HideSideBar = true;
                }

                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

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

            var groupUsers = group.GroupUsers.Select(x => new GroupUserViewModel { GroupUser = x, GroupUserStatus = GetUserStatusForGroup(x) });

            // Create the main view model for the Group
            var viewModel = new GroupMembersViewModel {

                PageIndex = pageIndex,
                TotalCount = group.GroupUsers.Count,
                TotalPages = (int)Math.Ceiling(group.GroupUsers.Count / (double)SettingsService.GetSettings().TopicsPerPage),
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
                viewModel.GroupUserStatus = GetUserStatusForGroup(user);
                viewModel.LoggedInUserRole = loggedOnUsersRole;
            }

            return PartialView("_ManageUsers", viewModel);
        }

        public virtual ActionResult ManageUser(Guid groupUserId)
        {
            // Get the Group
            var groupUser = _groupService.GetGroupUser(groupUserId);
            var roles = _roleService.AllRoles().Where(x => x.RoleName != Constants.GuestRoleName);
            var selectList = new List<SelectListItem>(roles.Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }));


            var viewModel = new GroupUserViewModel
            {
                GroupUser = groupUser,
                GroupUserStatus = GetUserStatusForGroup(groupUser),
                RoleSelectList = selectList,
                MemberRole = GetGroupMembershipRole(groupUser.Group.Id)

            };


            return View(viewModel);
        }

       public virtual ActionResult ApproveUser(Guid groupUserId, string slug)
        {
            _groupService.ApproveJoinGroup(groupUserId, LoggedOnReadOnlyUser.Id);
            return RedirectToRoute("GroupUrls", new { slug = slug, tab = Constants.GroupMembersTab });
        }
        public virtual ActionResult RejectUser(Guid groupUserId, string slug)
        {
            _groupService.RejectJoinGroup(groupUserId, LoggedOnReadOnlyUser.Id);
            return RedirectToRoute("GroupUrls", new { slug = slug, tab = Constants.GroupMembersTab });
        }

        [HttpPost]
        public virtual async Task<ActionResult> ManageUser(GroupUserViewModel model)
        {
            // Get the Group


            var groupUser = await _groupService.UpdateGroupUser(model.GroupUser);
            var roles = _roleService.AllRoles();
            var selectList = new List<SelectListItem>(roles.Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }));


            var viewModel = new GroupUserViewModel
            {
                GroupUser = groupUser,
                GroupUserStatus = GetUserStatusForGroup(groupUser),
                RoleSelectList = selectList,
                MemberRole = GetGroupMembershipRole(groupUser.Group.Id)

            };


            return View(viewModel);
        }

        private MembershipRole GetGroupMembershipRole(Guid groupId)
        {

            var role = _groupService.GetGroupRole(groupId, LoggedOnReadOnlyUser?.Id);
            if (LoggedOnReadOnlyUser == null)
                return role;

            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            return loggedOnUsersRole.RoleName == MvcForum.Core.Constants.Constants.AdminRoleName ? loggedOnUsersRole : role;
        }

    }
}