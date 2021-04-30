namespace MvcForum.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Application;
    using Application.CustomActionResults;
    using Application.ExtensionMethods;
    using Core;
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Activity;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Core.Models.General;
    using ViewModels.Home;

    public partial class HomeController : BaseController
    {
        private readonly IActivityService _activityService;
        private readonly IGroupService _GroupService;
        private readonly ITopicService _topicService;

        public HomeController(ILoggingService loggingService, IActivityService activityService,
            IMembershipService membershipService, ITopicService topicService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IGroupService GroupService,
            ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicService = topicService;
            _GroupService = GroupService;
            _activityService = activityService;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Leaderboard()
        {
            return View();
        }

        public virtual ActionResult Following()
        {
            return View();
        }

        public virtual ActionResult PostedIn()
        {
            return View();
        }

        public virtual ActionResult TermsAndConditions()
        {
            var settings = SettingsService.GetSettings();
            var viewModel = new TermsAndConditionsViewModel
            {
                Agree = false,
                TermsAndConditions = settings.TermsAndConditions
            };
            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult TermsAndConditions(TermsAndConditionsViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);

                var user = MembershipService.GetUser(loggedOnReadOnlyUser.Id);
                user.HasAgreedToTermsAndConditions = viewmodel.Agree;
                try
                {
                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                }
                return RedirectToAction("Index");
            }


            return View(viewmodel);
        }

        public virtual async Task<ActionResult> Activity(int? p)
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Set the page index
            var pageIndex = p ?? 1;

            // Get the topics
            var activities = await
                _activityService.GetPagedGroupedActivities(pageIndex,
                    SettingsService.GetSettings().ActivitiesPerPage, loggedOnUsersRole);

            // create the view model
            var viewModel = new AllRecentActivitiesViewModel
            {
                Activities = activities,
                PageIndex = pageIndex,
                TotalCount = activities.TotalCount
            };

            return View(viewModel);
        }

        [OutputCache(Duration = (int) CacheTimes.TwoHours)]
        public virtual ActionResult LatestRss()
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Allowed Groups for a guest - As that's all we want latest RSS to show
            var guestRole = RoleService.GetRole(Constants.GuestRoleName);
            var allowedGroups = _GroupService.GetAllowedGroups(guestRole);

            // get an rss lit ready
            var rssTopics = new List<RssItem>();

            // Get the latest topics
            var topics = _topicService.GetRecentRssTopics(50, allowedGroups);

            // Get all the Groups for this topic collection
            var Groups = topics.Select(x => x.Group).Distinct();

            // create permissions
            var permissions = new Dictionary<Group, PermissionSet>();

            // loop through the Groups and get the permissions
            foreach (var Group in Groups)
            {
                var permissionSet = RoleService.GetPermissions(Group, loggedOnUsersRole);
                permissions.Add(Group, permissionSet);
            }

            // Now loop through the topics and remove any that user does not have permission for
            foreach (var topic in topics)
            {
                // Get the permissions for this topic via its parent Group
                var permission = permissions[topic.Group];

                // Add only topics user has permission to
                if (!permission[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                {
                    if (topic.Posts.Any())
                    {
                        var firstOrDefault = topic.Posts.FirstOrDefault(x => x.IsTopicStarter);
                        if (firstOrDefault != null)
                        {
                            rssTopics.Add(new RssItem
                            {
                                Description = firstOrDefault.PostContent,
                                Link = topic.NiceUrl,
                                Title = topic.Name,
                                PublishedDate = topic.CreateDate
                            });
                        }
                    }
                }
            }

            return new RssResult(rssTopics, LocalizationService.GetResourceString("Rss.LatestActivity.Title"),
                LocalizationService.GetResourceString("Rss.LatestActivity.Description"));
        }

        [OutputCache(Duration = (int) CacheTimes.TwoHours)]
        public virtual ActionResult ActivityRss()
        {
            // get an rss lit ready
            var rssActivities = new List<RssItem>();

            var activities = _activityService.GetAll(50).OrderByDescending(x => x.ActivityMapped.Timestamp);

            var activityLink = Url.Action("Activity");

            // Now loop through the topics and remove any that user does not have permission for
            foreach (var activity in activities)
            {
                if (activity is BadgeActivity)
                {
                    var badgeActivity = activity as BadgeActivity;
                    rssActivities.Add(new RssItem
                    {
                        Description = badgeActivity.Badge.Description,
                        Title = string.Concat(badgeActivity.User.UserName, " ",
                            LocalizationService.GetResourceString("Activity.UserAwardedBadge"), " ",
                            badgeActivity.Badge.DisplayName, " ",
                            LocalizationService.GetResourceString("Activity.Badge")),
                        PublishedDate = badgeActivity.ActivityMapped.Timestamp,
                        RssImage = AppHelpers.ReturnBadgeUrl(badgeActivity.Badge.Image),
                        Link = activityLink
                    });
                }
                else if (activity is MemberJoinedActivity)
                {
                    var memberJoinedActivity = activity as MemberJoinedActivity;
                    rssActivities.Add(new RssItem
                    {
                        Description = string.Empty,
                        Title = LocalizationService.GetResourceString("Activity.UserJoined"),
                        PublishedDate = memberJoinedActivity.ActivityMapped.Timestamp,
                        RssImage = memberJoinedActivity.User.MemberImage(ForumConfiguration.Instance.GravatarPostSize),
                        Link = activityLink
                    });
                }
                else if (activity is ProfileUpdatedActivity)
                {
                    var profileUpdatedActivity = activity as ProfileUpdatedActivity;
                    rssActivities.Add(new RssItem
                    {
                        Description = string.Empty,
                        Title = LocalizationService.GetResourceString("Activity.ProfileUpdated"),
                        PublishedDate = profileUpdatedActivity.ActivityMapped.Timestamp,
                        RssImage = profileUpdatedActivity.User.MemberImage(ForumConfiguration.Instance.GravatarPostSize),
                        Link = activityLink
                    });
                }
            }

            return new RssResult(rssActivities, LocalizationService.GetResourceString("Rss.LatestActivity.Title"),
                LocalizationService.GetResourceString("Rss.LatestActivity.Description"));
        }

        [OutputCache(Duration = (int) CacheTimes.TwoHours)]
        public virtual ActionResult GoogleSitemap()
        {
            // Allowed Groups for a guest
            var guestRole = RoleService.GetRole(Constants.GuestRoleName);
            var allowedGroups = _GroupService.GetAllowedGroups(guestRole);

            // Get all topics that a guest has access to
            var allTopics = _topicService.GetAll(allowedGroups);

            // Sitemap holder
            var sitemap = new List<SitemapEntry>();

            // ##### TOPICS
            foreach (var topic in allTopics.Where(x => x.LastPost != null))
            {
                var sitemapEntry = new SitemapEntry
                {
                    Name = topic.Name,
                    Url = topic.NiceUrl,
                    LastUpdated = topic.LastPost.DateEdited,
                    ChangeFrequency = SiteMapChangeFreqency.Daily,
                    Priority = "0.6"
                };
                sitemap.Add(sitemapEntry);
            }

            return new GoogleSitemapResult(sitemap);
        }

        [OutputCache(Duration = (int) CacheTimes.TwoHours)]
        public virtual ActionResult GoogleMemberSitemap()
        {
            // get all members profiles
            var members = MembershipService.GetAll();

            // Sitemap holder
            var sitemap = new List<SitemapEntry>();

            // #### MEMBERS
            foreach (var member in members)
            {
                var sitemapEntry = new SitemapEntry
                {
                    Name = member.UserName,
                    Url = member.NiceUrl,
                    LastUpdated = member.CreateDate,
                    ChangeFrequency = SiteMapChangeFreqency.Weekly,
                    Priority = "0.4"
                };
                sitemap.Add(sitemapEntry);
            }

            return new GoogleSitemapResult(sitemap);
        }

        [OutputCache(Duration = (int) CacheTimes.TwoHours)]
        public virtual ActionResult GoogleGroupSitemap()
        {
            // Allowed Groups for a guest
            var guestRole = RoleService.GetRole(Constants.GuestRoleName);
            var allowedGroups = _GroupService.GetAllowedGroups(guestRole);

            // Sitemap holder
            var sitemap = new List<SitemapEntry>();

            // #### Groups
            foreach (var Group in allowedGroups)
            {
                // Get last post 
                var topic = Group.Topics.OrderByDescending(x => x.LastPost.DateEdited).FirstOrDefault();
                var sitemapEntry = new SitemapEntry
                {
                    Name = Group.Name,
                    Url = Group.NiceUrl,
                    LastUpdated = topic?.LastPost.DateEdited ?? Group.DateCreated,
                    ChangeFrequency = SiteMapChangeFreqency.Monthly
                };
                sitemap.Add(sitemapEntry);
            }

            //HttpResponse.RemoveOutputCacheItem(Url.Action("details", "product", new { id = 1234 }));

            return new GoogleSitemapResult(sitemap);
        }

        
    }
}