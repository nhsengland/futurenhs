namespace MvcForum.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Core.Models.General;
    using MvcForum.Core.Models.Enums;
    using ViewModels;
    using ViewModels.Mapping;
    using ViewModels.Post;

    [Authorize]
    public partial class PostController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IPostEditService _postEditService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly ITopicService _topicService;
        private readonly IVoteService _voteService;

        public PostController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ITopicService topicService,
            IPostService postService, ISettingsService settingsService, IGroupService GroupService,
            IReportService reportService, IVoteService voteService,
            IPostEditService postEditService, ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicService = topicService;
            _postService = postService;
            _groupService = GroupService;
            _reportService = reportService;
            _voteService = voteService;
            _postEditService = postEditService;
        }

        [HttpPost]
        public virtual async Task<ActionResult> CreatePost(CreateAjaxPostViewModel post)
        {
            var topic = _topicService.Get(post.Topic);

            var loggedOnUser = User.GetMembershipUser(MembershipService, false);

            var postPipelineResult = await _postService.Create(post.PostContent, topic, loggedOnUser, null, false, post.InReplyTo);

            if (!postPipelineResult.Successful)
            {
                // TODO - review how this is doene
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }

            // Post id so we know where to 'jump' to when redirecting, when replying Id of replying to otherwise new post Id
            var postId = post.InReplyTo == null ? postPipelineResult.EntityToProcess.Id : post.InReplyTo;

            //Check for moderation
            if (postPipelineResult.EntityToProcess.Pending == true)
            {
                return PartialView("_PostModeration");
            }

            // Get posts per page so we can calculate page to redirect to
            var postsPerPage = SettingsService.GetSettings().PostsPerPage;

            // Calculate the page to redirect to
            // First get all 'root' posts, i.e. no replies and not topic starter
            var rootPosts = topic.Posts.Where(x => x.InReplyTo == null).ToList().Where(x => !x.IsTopicStarter);

            // Default the post index to count of root posts
            var postIndex = rootPosts.Count();

            // If replying get the index of post replying to
            if (post.InReplyTo != null)
            {
                // Zero based so add one to get correct value
                postIndex = rootPosts.OrderBy(x => x.DateCreated).ToList().FindIndex(x => x.Id == post.InReplyTo) + 1;

                if (postIndex == 0)
                {
                    // would be 0 if reply to a reply etc, so go up the heirarchy until we get to root post
                    var rootInReplyTo = GetRootPostId(topic.Posts.ToList(), (Guid)post.InReplyTo);

                    // Set index of root post or default to 1 if not found
                    postIndex = rootInReplyTo != null ? rootPosts.OrderBy(x => x.DateCreated).ToList().FindIndex(x => x.Id == rootInReplyTo) + 1 : 1;
                }
            }

            // Calculate the page should redirect to based on postIndex and number of posts per page
            var page = (int)Math.Ceiling((double)postIndex / postsPerPage);

            return Redirect($"{topic.NiceUrl}?p={page}#{postId}");
        }

        [HttpPost]
        public virtual async Task<ActionResult> DeletePost(Guid id)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Got to get a lot of things here as we have to check permissions
            // Get the post
            var post = _postService.Get(id);
            var postId = post.Id;

            // get this so we know where to redirect after
            var isTopicStarter = post.IsTopicStarter;

            // Get the topic
            var topic = post.Topic;
            var topicUrl = topic.NiceUrl;

            // get the users permissions
            var permissions = RoleService.GetPermissions(topic.Group, loggedOnUsersRole);

            if (post.User.Id == LoggedOnReadOnlyUser?.Id ||
                permissions[ForumConfiguration.Instance.PermissionDeletePosts].IsTicked)
            {
                try
                {
                    // Delete post / topic
                    if (post.IsTopicStarter)
                    {
                        // Delete entire topic
                        var result = await _topicService.Delete(topic);
                        if (!result.Successful)
                        {
                            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = result.ProcessLog.FirstOrDefault(),
                                MessageType = GenericMessages.success
                            };

                            return Redirect(topic.NiceUrl);
                        }
                    }
                    else
                    {
                        // Deletes single post and associated data
                        var result = await _postService.Delete(post, false);
                        if (!result.Successful)
                        {
                            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = result.ProcessLog.FirstOrDefault(),
                                MessageType = GenericMessages.success
                            };

                            return Redirect(topic.NiceUrl);
                        }

                        // Remove in replyto's
                        var relatedPosts = _postService.GetReplyToPosts(postId);
                        foreach (var relatedPost in relatedPosts)
                        {
                            relatedPost.InReplyTo = null;
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                        MessageType = GenericMessages.danger
                    });
                    return Redirect(topicUrl);
                }
            }

            // Deleted successfully
            if (isTopicStarter)
            {
                // Redirect to root as this was a topic and deleted
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Topic.Deleted"),
                    MessageType = GenericMessages.success
                };
                return RedirectToAction("Index", "Home");
            }

            // Show message that post is deleted
            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = LocalizationService.GetResourceString("Post.Deleted"),
                MessageType = GenericMessages.success
            };

            return Redirect(topic.NiceUrl);
        }

        private ActionResult NoPermission(Topic topic)
        {
            // Trying to be a sneaky mo fo, so tell them
            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                MessageType = GenericMessages.danger
            };
            return Redirect(topic.NiceUrl);
        }

        public virtual ActionResult Report(Guid id)
        {
            if (SettingsService.GetSettings().EnableSpamReporting)
            {
                var post = _postService.Get(id);
                return View(new ReportPostViewModel { PostId = post.Id, PostCreatorUsername = post.User.UserName });
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }

        [HttpPost]
        public virtual ActionResult Report(ReportPostViewModel viewModel)
        {
            if (SettingsService.GetSettings().EnableSpamReporting)
            {
                User.GetMembershipUser(MembershipService);

                var post = _postService.Get(viewModel.PostId);
                var report = new Report
                {
                    Reason = viewModel.Reason,
                    ReportedPost = post,
                    Reporter = LoggedOnReadOnlyUser
                };
                _reportService.PostReport(report);

                try
                {
                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                }

                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Report.ReportSent"),
                    MessageType = GenericMessages.success
                };
                return this.Redirect(post.Topic.NiceUrl);
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }


        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult GetAllPostLikes(Guid id)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var post = _postService.Get(id);
            var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);
            var votes = _voteService.GetVotesByPosts(new List<Guid> { id });
            var viewModel = ViewModelMapping.CreatePostViewModel(post, votes[id], permissions, post.Topic,
                LoggedOnReadOnlyUser, SettingsService.GetSettings(), new List<Favourite>());
            var upVotes = viewModel.Votes.Where(x => x.Amount > 0).ToList();
            return View(upVotes);
        }


        public virtual ActionResult MovePost(Guid id)
        {
            User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

            // Firstly check if this is a post and they are allowed to move it
            var post = _postService.Get(id);
            if (post == null)
            {
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }

            var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            // Does the user have permission to this posts Group
            var cat = allowedGroups.FirstOrDefault(x => x.Id == post.Topic.Group.Id);
            if (cat == null)
            {
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }

            // Does this user have permission to move
            if (!permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
            {
                return NoPermission(post.Topic);
            }

            var topics = _topicService.GetAllSelectList(allowedGroups, 30);
            topics.Insert(0, new SelectListItem
            {
                Text = LocalizationService.GetResourceString("Topic.Choose"),
                Value = ""
            });

            var postViewModel = ViewModelMapping.CreatePostViewModel(post, post.Votes.ToList(), permissions, post.Topic,
                LoggedOnReadOnlyUser, SettingsService.GetSettings(), post.Favourites.ToList());
            postViewModel.MinimalPost = true;
            var viewModel = new MovePostViewModel
            {
                Post = postViewModel,
                PostId = post.Id,
                LatestTopics = topics,
                MoveReplyToPosts = true
            };
            return View(viewModel);
        }

        [HttpPost]
        public virtual async Task<ActionResult> MovePost(MovePostViewModel viewModel)
        {
            // Firstly check if this is a post and they are allowed to move it
            var post = _postService.Get(viewModel.PostId);
            if (post == null)
            {
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }

            var moveResult = await _postService.Move(post, viewModel.TopicId, viewModel.TopicTitle,
                viewModel.MoveReplyToPosts);
            if (moveResult.Successful)
            {
                // On Update redirect to the topic
                return RedirectToAction("Show", "Topic", new { slug = moveResult.EntityToProcess.Topic.Slug });
            }

            // Add a model error to show issue
            ModelState.AddModelError("", moveResult.ProcessLog.FirstOrDefault());

            // Sort the view model before sending back
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);
            var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);

            // Repopulate the topics
            var topics = _topicService.GetAllSelectList(allowedGroups, 30);
            topics.Insert(0, new SelectListItem
            {
                Text = LocalizationService.GetResourceString("Topic.Choose"),
                Value = ""
            });

            viewModel.LatestTopics = topics;
            viewModel.Post = ViewModelMapping.CreatePostViewModel(post, post.Votes.ToList(), permissions, 
                            post.Topic, LoggedOnReadOnlyUser, SettingsService.GetSettings(), post.Favourites.ToList());
            viewModel.Post.MinimalPost = true;
            viewModel.PostId = post.Id;

            return View(viewModel);
        }

        public virtual ActionResult GetPostEditHistory(Guid id)
        {
            var post = _postService.Get(id);
            if (post != null)
            {
                User.GetMembershipUser(MembershipService);
                var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

                // Check permissions
                var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);
                if (permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
                {
                    // Good to go
                    var postEdits = _postEditService.GetByPost(id);
                    var viewModel = new PostEditHistoryViewModel
                    {
                        PostEdits = postEdits
                    };
                    return PartialView(viewModel);
                }
            }

            return Content(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }

        public PartialViewResult GetPost(Post post)
        {
            var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);
            var permissions = RoleService.GetPermissions(post.Topic.Group, loggedOnUsersRole);

            var viewModel = ViewModelMapping.CreatePostViewModel(post, post.Votes.ToList(), permissions, post.Topic, LoggedOnReadOnlyUser, SettingsService.GetSettings(), post.Favourites.ToList());

            return PartialView("_post", viewModel);
        }

        /// <summary>
        /// Get root post Id from a list using in reply to. Recurses up until in reply
        /// to is null, at this point it's the root post.
        /// </summary>
        /// <param name="posts">List of posts.</param>
        /// <param name="inReplyTo">Guid InReplyTo.</param>
        /// <returns></returns>
        private Guid GetRootPostId(List<Post> posts, Guid inReplyTo)
        {
            if (posts == null)
            {
                return Guid.Empty;
            }

            var parent = posts.Where(x => x.Id == inReplyTo).FirstOrDefault();

            return parent.InReplyTo == null ? parent.Id : GetRootPostId(posts, (Guid)parent.InReplyTo);
        }
    }
}