namespace MvcForum.Web.Controllers
{
    using Core;
    using Core.Constants;
    using Core.Constants.UI;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Core.Models.General;
    using Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;
    using ViewModels.Breadcrumb;
    using ViewModels.ExtensionMethods;
    using ViewModels.Mapping;
    using ViewModels.Post;
    using ViewModels.Shared;
    using ViewModels.Topic;

    public partial class TopicController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IFavouriteService _favouriteService;
        private readonly INotificationService _notificationService;
        private readonly IPollService _pollService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly ITopicTagService _topicTagService;
        private readonly IVoteService _voteService;

        public TopicController(ILoggingService loggingService, IMembershipService membershipService,
            IRoleService roleService, ITopicService topicService, IPostService postService,
            IGroupService GroupService, ILocalizationService localizationService,
            ISettingsService settingsService, ITopicTagService topicTagService,
            IPollService pollService, IVoteService voteService, IFavouriteService favouriteService, ICacheService cacheService,
            IMvcForumContext context, INotificationService notificationService)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicService = topicService;
            _postService = postService;
            _groupService = GroupService;
            _topicTagService = topicTagService;
            _pollService = pollService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
        }


        [ChildActionOnly]
        [Authorize]
        public virtual PartialViewResult TopicsMemberHasPostedIn(int? p)
        {
            var loggedOnloggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            var allowedGroups = _groupService.GetAllowedGroups(loggedOnloggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var settings = SettingsService.GetSettings();
            // Set the page index
            var pageIndex = p ?? 1;

            // Get the topics
            var topics = Task.Run(() => _topicService.GetMembersActivity(pageIndex,
                settings.TopicsPerPage,
                ForumConfiguration.Instance.MembersActivityListSize,
                LoggedOnReadOnlyUser.Id,
                allowedGroups)).Result;

            // Get the Topic View Models
            var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, loggedOnloggedOnUsersRole,
                LoggedOnReadOnlyUser, allowedGroups, settings, _postService, _notificationService,
                _pollService, _voteService, _favouriteService);

            // create the view model
            var viewModel = new PostedInViewModel
            {
                Topics = topicViewModels,
                PageIndex = pageIndex,
                TotalCount = topics.TotalCount,
                TotalPages = topics.TotalPages
            };

            return PartialView("TopicsMemberHasPostedIn", viewModel);
        }

        [ChildActionOnly]
        [Authorize]
        public virtual PartialViewResult GetSubscribedTopics()
        {
            var viewModel = new List<TopicViewModel>();

            var loggedOnloggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);


            var allowedGroups = _groupService.GetAllowedGroups(loggedOnloggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var topicIds = LoggedOnReadOnlyUser.TopicNotifications.Select(x => x.Topic.Id).ToList();
            if (topicIds.Any())
            {
                var topics = _topicService.Get(topicIds, allowedGroups);

                // Get the Topic View Models
                viewModel = ViewModelMapping.CreateTopicViewModels(topics, RoleService, loggedOnloggedOnUsersRole,
                    LoggedOnReadOnlyUser, allowedGroups, SettingsService.GetSettings(), _postService,
                    _notificationService, _pollService, _voteService, _favouriteService);

                // Show the unsubscribe link
                foreach (var topicViewModel in viewModel)
                {
                    topicViewModel.ShowUnSubscribedLink = true;
                }
            }

            return PartialView("GetSubscribedTopics", viewModel);
        }

        [ChildActionOnly]
        public virtual PartialViewResult GetTopicBreadcrumb(Topic topic)
        {
            var loggedOnloggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            var Group = topic.Group;
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnloggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            var viewModel = new BreadcrumbViewModel
            {
                Groups = _groupService.GetGroupParents(Group, allowedGroups),
                Topic = topic
            };
            if (!viewModel.Groups.Any())
            {
                viewModel.Groups.Add(topic.Group);
            }
            return PartialView("GetGroupBreadcrumb", viewModel);
        }

        public virtual PartialViewResult CreateTopicButton(Guid groupId)
        {
            var loggedOnloggedOnUsersRole = GetGroupMembershipRole(groupId);

            var viewModel = new CreateTopicButtonViewModel
            {
                LoggedOnUser = LoggedOnReadOnlyUser
            };

            if (LoggedOnReadOnlyUser != null)
            {
                // Add all Groups to a permission set
                var allGroups = _groupService.GetAll(LoggedOnReadOnlyUser?.Id);

                foreach (var Group in allGroups)
                {
                    // Now check to see if they have access to any Groups
                    // if so, check they are allowed to create topics - If no to either set to false
                    viewModel.UserCanPostTopics = false;
                    var permissionSet = RoleService.GetPermissions(Group, loggedOnloggedOnUsersRole);
                    if (permissionSet[ForumConfiguration.Instance.PermissionCreateTopics].IsTicked)
                    {
                        viewModel.UserCanPostTopics = true;
                        viewModel.GroupId = groupId;
                        break;
                    }
                }
            }
            return PartialView(viewModel);
        }

        [HttpPost]
        [Authorize]
        public virtual JsonResult CheckTopicCreatePermissions(Guid catId)
        {
            if (Request.IsAjaxRequest())
            {
                User.GetMembershipUser(MembershipService);
                var loggedOnloggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
                var Group = _groupService.Get(catId);
                var permissionSet = RoleService.GetPermissions(Group, loggedOnloggedOnUsersRole);
                var model = GetCheckCreateTopicPermissions(permissionSet);
                return Json(model);
            }
            return null;
        }

        #region Create / Edit Helper Methods

        private static CheckCreateTopicPermissions GetCheckCreateTopicPermissions(PermissionSet permissionSet)
        {
            var model = new CheckCreateTopicPermissions();

            if (permissionSet[ForumConfiguration.Instance.PermissionCreateStickyTopics].IsTicked)
            {
                model.CanStickyTopic = true;
            }

            if (permissionSet[ForumConfiguration.Instance.PermissionLockTopics].IsTicked)
            {
                model.CanLockTopic = true;
            }

            if (permissionSet[ForumConfiguration.Instance.PermissionAttachFiles].IsTicked)
            {
                model.CanUploadFiles = true;
            }

            if (permissionSet[ForumConfiguration.Instance.PermissionCreatePolls].IsTicked)
            {
                model.CanCreatePolls = true;
            }

            if (permissionSet[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked)
            {
                model.CanInsertImages = true;
            }

            if (permissionSet[ForumConfiguration.Instance.PermissionCreateTags].IsTicked)
            {
                model.CanCreateTags = true;
            }
            return model;
        }

        #endregion

        private CreateEditTopicViewModel PrePareCreateEditTopicViewModel(List<Group> allowedGroups)
        {
            var userIsAdmin = User.IsInRole(Constants.AdminRoleName);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var permissions = RoleService.GetPermissions(null, loggedOnUsersRole);
            var canInsertImages = userIsAdmin;
            if (!canInsertImages)
            {
                canInsertImages = permissions[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked;
            }
            return new CreateEditTopicViewModel
            {
                SubscribeToTopic = true,
                Groups = _groupService.GetBaseSelectListGroups(allowedGroups, LoggedOnReadOnlyUser?.Id),
                OptionalPermissions = new CheckCreateTopicPermissions
                {
                    CanLockTopic = userIsAdmin,
                    CanStickyTopic = userIsAdmin,
                    CanUploadFiles = userIsAdmin,
                    CanCreatePolls = userIsAdmin,
                    CanInsertImages = canInsertImages,
                    CanCreateTags = userIsAdmin
                },
                PollAnswers = new List<PollAnswer>(),
                IsTopicStarter = true,
                PollCloseAfterDays = 0
            };
        }

        private List<Group> AllowedCreateGroups(MembershipRole loggedOnUsersRole)
        {
            var allowedAccessGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var allowedCreateTopicGroups = _groupService.GetAllowedGroups(loggedOnUsersRole,
                ForumConfiguration.Instance.PermissionCreateTopics, LoggedOnReadOnlyUser?.Id);
            var allowedCreateTopicGroupIds = allowedCreateTopicGroups.Select(x => x.Id);
            if (allowedAccessGroups.Any())
            {
                allowedAccessGroups.RemoveAll(x => allowedCreateTopicGroupIds.Contains(x.Id));
                allowedAccessGroups.RemoveAll(x =>
                    loggedOnUsersRole.RoleName != Constants.AdminRoleName && x.IsLocked);
            }
            return allowedAccessGroups;
        }

        // TODO Duplicated code from groups, we need to refactor all of this into one place.
        public TabViewModel GetGroupTabsModel(string slug)
        {
            var forumTab = new Tab
            {
                Name = "GroupTabs.Forum",
                Order = 2,
                Icon = Icons.Forum,
                Url = $"{Url.RouteUrl("GroupUrls", new {slug = slug, tab = Constants.GroupForumTab})}",
                Active = true
            };

            var membersTab = new Tab
            {
                Name = "GroupTabs.Members",
                Order = 3,
                Icon = Icons.Members,
                Url = $"{Url.RouteUrl("GroupUrls", new {slug = slug, tab = Constants.GroupMembersTab})}"
            };

            var homeTab = new Tab
            {
                Name = "GroupTabs.Home",
                Order = 1,
                Icon = Icons.Home,
                Url = $"{Url.RouteUrl("GroupUrls", new {slug = slug, tab = UrlParameter.Optional})}"
            };

            var tabsViewModel = new TabViewModel { Tabs = new List<Tab> { homeTab, forumTab, membersTab } };

            return tabsViewModel;
        }

        /// <summary>
        ///     Create topic view
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult Create(Guid groupId)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = GetGroupMembershipRole(groupId);
            var allowedAccessGroups = AllowedCreateGroups(loggedOnUsersRole);

            var group = _groupService.Get(groupId);
            var pageHeader = new PageViewModel();
            pageHeader.Name = group.Name;
            pageHeader.Description = group.Description;
            pageHeader.Colour = group.Colour;
            pageHeader.HeaderTabs = GetGroupTabsModel(group.Slug);
            pageHeader.Image = group.Image;
            pageHeader.Id = group.Id;

            ViewBag.PageHeader = pageHeader;

            if (allowedAccessGroups.Any() && LoggedOnReadOnlyUser.DisablePosting != true)
            {
                var viewModel = PrePareCreateEditTopicViewModel(allowedAccessGroups);
                viewModel.Group = groupId;
                viewModel.GroupSlug = group.Slug;
                return View(viewModel);
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        /// <summary>
        ///     Creates a topic via the pipeline system
        /// </summary>
        /// <param name="topicViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create(CreateEditTopicViewModel topicViewModel)
        {
            // Get the user and roles
            var loggedOnUser = User.GetMembershipUser(MembershipService, false);
            var loggedOnUsersRole = GetGroupMembershipRole(topicViewModel.Group);

            // Get the Group
            var group = _groupService.Get(topicViewModel.Group);

            // First check this user is allowed to create topics in this Group
            var permissions = RoleService.GetPermissions(group, loggedOnUsersRole);



            var pageHeader = new PageViewModel();
            pageHeader.Name = group.Name;
            pageHeader.Description = group.Description;
            pageHeader.Colour = group.Colour;
            pageHeader.HeaderTabs = GetGroupTabsModel(group.Slug);
            pageHeader.Image = group.Image;
            pageHeader.Id = group.Id;

            ViewBag.PageHeader = pageHeader;

            // Now we have the Group and permissionSet - Populate the optional permissions 
            // This is just in case the viewModel is return back to the view also sort the allowedGroups
            topicViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);
            topicViewModel.Groups = _groupService.GetBaseSelectListGroups(AllowedCreateGroups(loggedOnUsersRole), LoggedOnReadOnlyUser?.Id);
            topicViewModel.GroupSlug = group.Slug;
            topicViewModel.IsTopicStarter = true;
            if (topicViewModel.PollAnswers == null)
            {
                topicViewModel.PollAnswers = new List<PollAnswer>();
            }

            if (ModelState.IsValid)
            {
                // See if the user has actually added some content to the topic
                if (string.IsNullOrWhiteSpace(topicViewModel.Content))
                {
                    ModelState.AddModelError("Content",
                        LocalizationService.GetResourceString("This field is required"));
                }
                else
                {
                    // Map the new topic (Pass null for new topic)
                    var topic = topicViewModel.ToTopic(group, loggedOnUser, null);

                    // Run the create pipeline
                    var createPipeLine = await _topicService.Create(topic, topicViewModel.Files, topicViewModel.Tags,
                        topicViewModel.SubscribeToTopic, topicViewModel.Content, null);
                    if (createPipeLine.Successful == false)
                    {
                        // TODO - Not sure on this?
                        // Remove the topic if unsuccessful, as we may have saved some items.
                        await _topicService.Delete(createPipeLine.EntityToProcess);

                        // Tell the user the topic is awaiting moderation
                        ModelState.AddModelError(string.Empty, createPipeLine.ProcessLog.FirstOrDefault());
                        return View(topicViewModel);
                    }

                    if (createPipeLine.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
                    {
                        var moderate = createPipeLine.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;
                        if (moderate == true)
                        {
                            // Tell the user the topic is awaiting moderation
                            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
                                MessageType = GenericMessages.info
                            };

                            return RedirectToAction("Index", "Home");
                        }
                    }

                    // Redirect to the newly created topic
                    return Redirect($"{topic.NiceUrl}?postbadges=true");
                }
            }

            return View(topicViewModel);
        }


        [Authorize]
        public virtual ActionResult EditPostTopic(Guid id)
        {
            // Get the post
            var post = _postService.Get(id);

            // Get the topic
            var topic = post.Topic;

            // Get the current logged on user
            User.GetMembershipUser(MembershipService);
            var loggedOnloggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // get the users permissions
            var permissions = RoleService.GetPermissions(topic.Group, loggedOnloggedOnUsersRole);

            // Is the user allowed to edit this post
            if (post.User.Id == LoggedOnReadOnlyUser?.Id ||
                permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
            {
                // Get the allowed Groups for this user
                var allowedAccessGroups = _groupService.GetAllowedGroups(loggedOnloggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
                var allowedCreateTopicGroups =
                    _groupService.GetAllowedGroups(loggedOnloggedOnUsersRole,
                        ForumConfiguration.Instance.PermissionCreateTopics, LoggedOnReadOnlyUser?.Id);
                var allowedCreateTopicGroupIds = allowedCreateTopicGroups.Select(x => x.Id);

                // If this user hasn't got any allowed cats OR they are not allowed to post then abandon
                if (allowedAccessGroups.Any() && LoggedOnReadOnlyUser.DisablePosting != true)
                {
                    // Create the model for just the post
                    var viewModel = new CreateEditTopicViewModel
                    {
                        Content = post.PostContent,
                        Id = post.Id,
                        Group = topic.Group.Id,
                        Name = topic.Name,
                        TopicId = topic.Id,
                        OptionalPermissions = GetCheckCreateTopicPermissions(permissions),
                        IsPostEdit = true
                    };

                    // Now check if this is a topic starter, if so add the rest of the field
                    if (post.IsTopicStarter)
                    {
                        // Remove all Groups that don't have create topic permission
                        allowedAccessGroups.RemoveAll(x => allowedCreateTopicGroupIds.Contains(x.Id));

                        // See if this user is subscribed to this topic
                        var topicNotifications =
                            _notificationService.GetTopicNotificationsByUserAndTopic(LoggedOnReadOnlyUser, topic);

                        // Populate the properties we can
                        viewModel.IsLocked = topic.IsLocked;
                        viewModel.IsSticky = topic.IsSticky;
                        viewModel.IsTopicStarter = post.IsTopicStarter;
                        viewModel.SubscribeToTopic = topicNotifications.Any();
                        viewModel.Groups =
                            _groupService.GetBaseSelectListGroups(allowedAccessGroups, LoggedOnReadOnlyUser?.Id);

                        // Tags - Populate from the topic
                        if (topic.Tags.Any())
                        {
                            viewModel.Tags = string.Join<string>(",", topic.Tags.Select(x => x.Tag));
                        }

                        // Populate the poll answers
                        if (topic.Poll != null && topic.Poll.PollAnswers.Any())
                        {
                            // Has a poll so add it to the view model
                            viewModel.PollAnswers = topic.Poll.PollAnswers;
                            viewModel.PollCloseAfterDays = topic.Poll.ClosePollAfterDays ?? 0;
                        }

                        // It's a topic
                        viewModel.IsPostEdit = false;
                    }

                    // Return the edit view
                    return View(viewModel);
                }
            }

            // If we get here the user has no permission to try and edit the post
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> EditPostTopic(CreateEditTopicViewModel editPostViewModel)
        {
            // Get the current user and role
            var loggedOnUser = User.GetMembershipUser(MembershipService, false);
            var loggedOnUsersRole = loggedOnUser.GetRole(RoleService, false);

            // Get the Group
            var Group = _groupService.Get(editPostViewModel.Group);

            // Get all the permissions for this user
            var permissions = RoleService.GetPermissions(Group, loggedOnUsersRole);

            // Now we have the Group and permissionSet - Populate the optional permissions 
            // This is just in case the viewModel is return back to the view also sort the allowedGroups
            // Get the allowed Groups for this user
            var allowedAccessGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var allowedCreateTopicGroups = _groupService.GetAllowedGroups(loggedOnUsersRole,
                ForumConfiguration.Instance.PermissionCreateTopics, LoggedOnReadOnlyUser?.Id);
            var allowedCreateTopicGroupIds = allowedCreateTopicGroups.Select(x => x.Id);

            // TODO ??? Is this correct ??
            allowedAccessGroups.RemoveAll(x => allowedCreateTopicGroupIds.Contains(x.Id));

            // Set the Groups
            editPostViewModel.Groups = _groupService.GetBaseSelectListGroups(allowedAccessGroups, LoggedOnReadOnlyUser?.Id);

            // Get the users permissions for the topic
            editPostViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);

            // See if this is a topic starter or not
            editPostViewModel.IsTopicStarter = editPostViewModel.Id == Guid.Empty;

            // IS the model valid
            if (ModelState.IsValid)
            {
                // Got to get a lot of things here as we have to check permissions
                // Get the post
                var originalPost = _postService.Get(editPostViewModel.Id);

                // Get the topic
                var originalTopic = originalPost.Topic;

                // See if the user has actually added some content to the topic
                if (string.IsNullOrWhiteSpace(editPostViewModel.Content))
                {
                    ModelState.AddModelError(string.Empty,
                        LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
                else
                {

                    bool successful;
                    bool? moderate = false;
                    string message;

                    if (editPostViewModel.IsPostEdit)
                    {
                        var editPostPipe = await _postService.Edit(originalPost, editPostViewModel.Files,
                            originalPost.IsTopicStarter, string.Empty, editPostViewModel.Content);

                        successful = editPostPipe.Successful;
                        message = editPostPipe.ProcessLog.FirstOrDefault();
                        if (editPostPipe.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
                        {
                            moderate = editPostPipe.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;
                        }
                    }
                    else
                    {
                        // Map the new topic (Pass null for new topic)
                        var topic = editPostViewModel.ToTopic(Group, loggedOnUser, originalTopic);

                        // Run the create pipeline
                        var editPipeLine = await _topicService.Edit(topic, editPostViewModel.Files,
                            editPostViewModel.Tags, editPostViewModel.SubscribeToTopic, editPostViewModel.Content,
                            editPostViewModel.Name, editPostViewModel.PollAnswers, editPostViewModel.PollCloseAfterDays);

                        successful = editPipeLine.Successful;
                        message = editPipeLine.ProcessLog.FirstOrDefault();
                        if (editPipeLine.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
                        {
                            moderate = editPipeLine.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;
                        }
                    }


                    // Check if successful
                    if (successful == false)
                    {
                        // Tell the user the topic is awaiting moderation
                        ModelState.AddModelError(string.Empty, message);
                        return View(editPostViewModel);
                    }


                    if (moderate == true)
                    {
                        // Tell the user the topic is awaiting moderation
                        TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
                            MessageType = GenericMessages.info
                        };

                        return RedirectToAction("Index", "Home");
                    }


                    // Redirect to the newly created topic
                    return Redirect($"{originalTopic.NiceUrl}?postbadges=true");
                }
            }

            return View(editPostViewModel);
        }

        public virtual async Task<ActionResult> Show(string slug, int p = 1, Guid? threadId = null)
        {
            // Set the page index
            var pageIndex = p <= 0 ? 1 : p;

            // Get the topic
            var topic = _topicService.GetTopicBySlug(slug);
            
            ViewBag.HideSideBar = true;

            if (topic != null)
            {
                var loggedOnUsersRole = GetGroupMembershipRole(topic.Group.Id);
                var settings = SettingsService.GetSettings();

                // Note: Don't use topic.Posts as its not a very efficient SQL statement
                // Use the post service to get them as it includes other used entities in one
                // statement rather than loads of sql selects

                var sortQuerystring = Request.QueryString[Constants.PostOrderBy];
                var orderBy = !string.IsNullOrWhiteSpace(sortQuerystring)
                    ? EnumUtils.ReturnEnumValueFromString<PostOrderBy>(sortQuerystring)
                    : PostOrderBy.Standard;

                // Store the amount per page
                var amountPerPage = settings.PostsPerPage;

                if (sortQuerystring == Constants.AllPosts)
                {
                    // Overide to show all posts
                    amountPerPage = int.MaxValue;
                }

                // Get the posts
                var posts = await _postService.GetPagedPostsByTopic(pageIndex,
                    amountPerPage,
                    int.MaxValue,
                    topic.Id,
                    orderBy);

                // Get the topic starter post
                var starterPost = _postService.GetTopicStarterPost(topic.Id);

                // Get the permissions for the Group that this topic is in
                var permissions = RoleService.GetPermissions(topic.Group, loggedOnUsersRole);

                // If this user doesn't have access to this topic then
                // redirect with message
                if (permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                {
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                }

                // Set editor permissions
                ViewBag.ImageUploadType = permissions[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked
                    ? "forumimageinsert"
                    : "image";

                var postIds = posts.Select(x => x.Id).ToList();

                var votes = _voteService.GetVotesByPosts(postIds);

                var favourites = _favouriteService.GetAllPostFavourites(postIds);

                var viewModel = ViewModelMapping.CreateTopicViewModel(topic, permissions, posts, postIds,
                    starterPost, posts.PageIndex, posts.TotalCount, posts.TotalPages, LoggedOnReadOnlyUser,
                    settings, _notificationService, _pollService, votes, favourites, true);

                // Set the details for the logged in user.
                var currentUser = User.GetMembershipUser(MembershipService);

                viewModel.LoggedInUsersName = "Unknown";
                viewModel.LoggedInUsersUrl = string.Empty;

                if (currentUser != null)
                {
                    viewModel.LoggedInUsersName = currentUser.GetFullName();
                    viewModel.LoggedInUsersUrl = currentUser.NiceUrl;
                }

                viewModel.TotalComments = _postService.TopicPostCount(viewModel.Topic.Id);
                foreach (var post in viewModel.Posts) 
                {
                    post.PageIndex = pageIndex;
                    post.Replies = new PaginatedList<Post>(new List<Post>(), _postService.GetPostCountForThread(post.Post.Id) - 1, 0, SettingsService.GetSettings().PostsPerPage);

                    if (threadId.HasValue && post.Post.Id == threadId.Value)
                    {
                        var replyCount = _postService.GetPostCountForThread(post.Post.Id);
                        post.Replies = new PaginatedList<Post>(new List<Post>(), replyCount, (int)Math.Ceiling(replyCount/(double)ForumConfiguration.Instance.PagingRepliesSize), ForumConfiguration.Instance.PagingRepliesSize);
                        post.IsFocusThread = true;
                    }
                    else
                    {
                        post.Replies = new PaginatedList<Post>(new List<Post>(), _postService.GetPostCountForThread(post.Post.Id) - 1, 0, ForumConfiguration.Instance.PagingRepliesSize);
                        post.IsFocusThread = false;
                    }

                    // TODO set the page index on the other replies when show more button is working (non JS already done)

                    var latestPost = _postService.GetLatestPostForThread(post.Post.Id, orderBy);
                    if (latestPost != null)
                    {
                        post.LatestReply = ViewModelMapping.CreatePostViewModel(latestPost, latestPost.Votes.ToList(),
                            permissions, topic, LoggedOnReadOnlyUser,
                            settings, new List<Favourite>());

                        post.LatestReply.PageIndex = pageIndex;
                    }
                }

                // If there is a quote querystring
                var quote = Request["quote"];
                if (!string.IsNullOrWhiteSpace(quote))
                {
                    try
                    {
                        // Got a quote
                        var postToQuote = _postService.Get(new Guid(quote));
                        viewModel.QuotedPost = postToQuote.PostContent;
                        viewModel.ReplyTo = postToQuote.Id;
                        viewModel.ReplyToUsername = postToQuote.User.UserName;
                        viewModel.ReplyToUsernameUrl = postToQuote.User.NiceUrl;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                var reply = Request["reply"];
                if (!string.IsNullOrWhiteSpace(reply))
                {
                    try
                    {
                        // Set the reply
                        var toReply = _postService.Get(new Guid(reply));
                        viewModel.ReplyTo = toReply.Id;
                        viewModel.ReplyToUsername = toReply.User.UserName;
                        viewModel.ReplyToUsernameUrl = toReply.User.NiceUrl;
                        viewModel.Thread = toReply.ThreadId != null ? toReply.ThreadId : toReply.Id;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                var updateDatabase = false;

                // User has permission lets update the topic view count
                // but only if this topic doesn't belong to the user looking at it
                var addView = !(User.Identity.IsAuthenticated && LoggedOnReadOnlyUser?.Id == topic.User.Id);
                if (addView)
                {
                    updateDatabase = true;
                }

                // Check the poll - To see if it has one, and whether it needs to be closed.
                if (viewModel.Poll?.Poll?.ClosePollAfterDays != null &&
                    viewModel.Poll.Poll.ClosePollAfterDays > 0 &&
                    !viewModel.Poll.Poll.IsClosed)
                {
                    // Check the date the topic was created
                    var endDate =
                        viewModel.Poll.Poll.DateCreated.AddDays((int)viewModel.Poll.Poll.ClosePollAfterDays);
                    if (DateTime.UtcNow > endDate)
                    {
                        topic.Poll.IsClosed = true;
                        viewModel.Topic.Poll.IsClosed = true;
                        updateDatabase = true;
                    }
                }

                if (!BotUtils.UserIsBot() && updateDatabase)
                {
                    if (addView)
                    {
                        // Increase the topic views
                        topic.Views = topic.Views + 1;
                    }

                    try
                    {
                        Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                // Create the view model for new post here rather than in view so we can set error if any (also nicer from a view perspective)
                viewModel.NewPostViewModel = 
                    new CreateAjaxPostViewModel()
                    {
                        Topic = viewModel.Topic.Id,
                        DisablePosting = viewModel.DisablePosting,
                        PostContent = viewModel.QuotedPost,
                        InReplyTo = viewModel.ReplyTo,
                        ReplyToUsername = viewModel.ReplyToUsername,
                        CurrentUser = viewModel.LoggedInUsersName,
                        CurrentUserUrl = viewModel.LoggedInUsersUrl,
                        ReplyToUsernameUrl = viewModel.ReplyToUsernameUrl,
                        Thread = viewModel.Thread
                    };

                if (TempData["NewPostError"] != null && !string.IsNullOrWhiteSpace(TempData["NewPostError"].ToString()))
                {
                    viewModel.NewPostViewModel.Error = TempData["NewPostError"].ToString();
                    TempData["NewPostError"] = null;
                }

                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }

        [HttpPost]
        public virtual async Task<PartialViewResult> AjaxMorePosts(GetMorePostsViewModel getMorePostsViewModel)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Get the topic
            var topic = _topicService.Get(getMorePostsViewModel.TopicId);
            var settings = SettingsService.GetSettings();

            // Get the permissions for the Group that this topic is in
            var permissions = RoleService.GetPermissions(topic.Group, loggedOnUsersRole);

            // If this user doesn't have access to this topic then just return nothing
            if (permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                return null;
            }

            var orderBy = !string.IsNullOrWhiteSpace(getMorePostsViewModel.Order)
                ? EnumUtils.ReturnEnumValueFromString<PostOrderBy>(getMorePostsViewModel.Order)
                : PostOrderBy.Standard;

            var posts = Task.Run(() => _postService.GetPagedPostsByTopic(getMorePostsViewModel.PageIndex,
                settings.PostsPerPage, int.MaxValue, topic.Id, orderBy)).Result;
            var postIds = posts.Select(x => x.Id).ToList();
            var votes = _voteService.GetVotesByPosts(postIds);
            var favs = _favouriteService.GetAllPostFavourites(postIds);
            var viewModel = new ShowMorePostsViewModel
            {
                Posts = ViewModelMapping.CreatePostViewModels(posts, votes, permissions, topic,
                    LoggedOnReadOnlyUser, settings, favs),
                Topic = topic,
                Permissions = permissions
            };

            foreach (var post in viewModel.Posts)
            {
                post.Replies = new PaginatedList<Post>(new List<Post>(), _postService.GetPostCountForThread(post.Post.Id) - 1, 0, ForumConfiguration.Instance.PagingRepliesSize);
                var latestPost = _postService.GetLatestPostForThread(post.Post.Id, orderBy);
                if (latestPost != null)
                {
                    post.LatestReply = ViewModelMapping.CreatePostViewModel(latestPost, latestPost.Votes.ToList(),
                        permissions, topic, LoggedOnReadOnlyUser,
                        settings, new List<Favourite>());
                }
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public virtual PartialViewResult AjaxMorePostsForThread(GetMorePostsViewModel getMorePostsViewModel)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Get the topic
            var thread = _postService.Get(getMorePostsViewModel.TopicId);
            var settings = SettingsService.GetSettings();

            // Get the permissions for the Group that this topic is in
            var permissions = RoleService.GetPermissions(thread.Topic.Group, loggedOnUsersRole);

            // If this user doesn't have access to this topic then just return nothing
            if (permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
            {
                return null;
            }

            var orderBy = !string.IsNullOrWhiteSpace(getMorePostsViewModel.Order)
                ? EnumUtils.ReturnEnumValueFromString<PostOrderBy>(getMorePostsViewModel.Order)
                : PostOrderBy.Standard;

            var posts = Task.Run(() => _postService.GetPagedPostsByThread(getMorePostsViewModel.PageIndex,
                ForumConfiguration.Instance.PagingRepliesSize, int.MaxValue, getMorePostsViewModel.TopicId, orderBy)).Result;
            var postIds = posts.Select(x => x.Id).ToList();
            var votes = _voteService.GetVotesByPosts(postIds);
            var favs = _favouriteService.GetAllPostFavourites(postIds);
            var viewModel = new ShowMorePostsViewModel
            {
                Posts = ViewModelMapping.CreatePostViewModels(posts, votes, permissions, thread.Topic,
                    LoggedOnReadOnlyUser, settings, favs),
                Topic = thread.Topic,
                Permissions = permissions
            };

            return PartialView(viewModel);
        }

        public PartialViewResult GetNoScriptRepliesForThread(Post thread, int p = 1)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Skip the first one as we already get this as a latest post
            List<Post> posts = _postService.GetPostsByThread(thread.Id).Skip(1).Take(ForumConfiguration.Instance.NoScriptReplyCount).ToList();

            var permissions = RoleService.GetPermissions(thread.Topic.Group, loggedOnUsersRole);
            var postIds = posts.Select(x => x.Id).ToList();
            var votes = _voteService.GetVotesByPosts(postIds);
            var favs = _favouriteService.GetAllPostFavourites(postIds);
            var viewModel = new ShowMorePostsViewModel
            {
                Posts = ViewModelMapping.CreatePostViewModels(posts, votes, permissions, thread.Topic,
                    LoggedOnReadOnlyUser, SettingsService.GetSettings(), favs),
                PageIndex = p,
            };

            foreach (var post in viewModel.Posts)
            {
                post.PageIndex = p;
            }

            return PartialView("AjaxMorePosts", viewModel);
        }

        public PartialViewResult GetAllRepliesForThread(Post thread)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Skip the first one as we already get this as a latest post
            List<Post> posts = _postService.GetPostsByThread(thread.Id).ToList();

            var permissions = RoleService.GetPermissions(thread.Topic.Group, loggedOnUsersRole);
            var postIds = posts.Select(x => x.Id).ToList();
            var votes = _voteService.GetVotesByPosts(postIds);
            var favs = _favouriteService.GetAllPostFavourites(postIds);
            var viewModel = new ShowMorePostsViewModel
            {
                Posts = ViewModelMapping.CreatePostViewModels(posts, votes, permissions, thread.Topic,
                    LoggedOnReadOnlyUser, SettingsService.GetSettings(), favs)
            };

            return PartialView("AjaxMorePosts", viewModel);
        }

        public virtual async Task<ActionResult> TopicsByTag(string tag, int? p)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var settings = SettingsService.GetSettings();
            var tagModel = _topicTagService.Get(tag);
            if (tag != null)
            {
                // Set the page index
                var pageIndex = p ?? 1;

                // Get the topics
                var topics = await _topicService.GetPagedTopicsByTag(pageIndex,
                    settings.TopicsPerPage,
                    int.MaxValue,
                    tag, allowedGroups);

                // See if the user has subscribed to this topic or not
                var isSubscribed = User.Identity.IsAuthenticated &&
                                   _notificationService.GetTagNotificationsByUserAndTag(LoggedOnReadOnlyUser, tagModel)
                                       .Any();

                // Get the Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, loggedOnUsersRole,
                    LoggedOnReadOnlyUser, allowedGroups, settings, _postService, _notificationService,
                    _pollService, _voteService, _favouriteService);

                // create the view model
                var viewModel = new TagTopicsViewModel();
                viewModel.Topics = topicViewModels;
                viewModel.PageIndex = pageIndex;
                viewModel.TotalCount = topics.TotalCount;
                viewModel.TotalPages = topics.TotalPages;
                viewModel.Tag = tag;
                viewModel.IsSubscribed = isSubscribed;
                viewModel.TagId = tagModel.Id;


                return View(viewModel);
            }

            return ErrorToHomePage("No Such Tag");
        }

        [HttpPost]
        public virtual PartialViewResult GetSimilarTopics(string searchTerm)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Returns the formatted string to search on
            var formattedSearchTerm = StringUtils.ReturnSearchString(searchTerm);
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            IList<Topic> topics = null;
            try
            {
                var searchResults = _topicService.SearchTopics(ForumConfiguration.Instance.SimilarTopicsListSize,
                    formattedSearchTerm, allowedGroups);
                if (searchResults != null)
                {
                    topics = searchResults;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
            }

            // Pass the list to the partial view
            return PartialView(topics);
        }

        [ChildActionOnly]
        public virtual ActionResult LatestTopics(int? p)
        {
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);
            var settings = SettingsService.GetSettings();

            // Set the page index
            var pageIndex = p ?? 1;

            // Get the topics
            var topics = Task.Run(() => _topicService.GetRecentTopics(pageIndex,
                settings.TopicsPerPage,
                ForumConfiguration.Instance.ActiveTopicsListSize,
                allowedGroups)).Result;

            // Get the Topic View Models
            var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, loggedOnUsersRole,
                LoggedOnReadOnlyUser, allowedGroups, settings, _postService, _notificationService,
                _pollService, _voteService, _favouriteService);

            // create the view model
            var viewModel = new ActiveTopicsViewModel
            {
                Topics = topicViewModels,
                PageIndex = pageIndex,
                TotalCount = topics.TotalCount,
                TotalPages = topics.TotalPages
            };

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public virtual ActionResult HotTopics(DateTime? from, DateTime? to, int? amountToShow)
        {
            if (amountToShow == null)
            {
                amountToShow = 5;
            }

            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            var fromString = from != null ? Convert.ToDateTime(from).ToShortDateString() : null;
            var toString = to != null ? Convert.ToDateTime(to).ToShortDateString() : null;

            var cacheKey = string.Concat("HotTopics", loggedOnUsersRole.Id, fromString, toString, amountToShow);
            var viewModel = CacheService.Get<HotTopicsViewModel>(cacheKey);
            if (viewModel == null)
            {
                // Allowed Groups
                var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

                // Get the topics
                var topics = _topicService.GetPopularTopics(from, to, allowedGroups, (int)amountToShow);

                // Get the Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService,
                    loggedOnUsersRole, LoggedOnReadOnlyUser, allowedGroups, SettingsService.GetSettings(),
                    _postService, _notificationService, _pollService, _voteService, _favouriteService);

                // create the view model
                viewModel = new HotTopicsViewModel
                {
                    Topics = topicViewModels
                };
                CacheService.Set(cacheKey, viewModel, CacheTimes.TwoHours);
            }

            return PartialView(viewModel);
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