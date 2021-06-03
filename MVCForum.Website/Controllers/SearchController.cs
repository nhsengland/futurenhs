namespace MvcForum.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using ViewModels.Mapping;
    using ViewModels.Search;

    public partial class SearchController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IFavouriteService _favouriteService;
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;

        public SearchController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IPostService postService, IVoteService voteService, IFavouriteService favouriteService,
            IGroupService GroupService, ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _postService = postService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _groupService = GroupService;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index(int? p, string siteSearch)
        {
            if (!string.IsNullOrWhiteSpace(siteSearch))
            {
                if (!string.IsNullOrWhiteSpace(siteSearch))
                {
                    siteSearch = siteSearch.Trim();
                }

                var loggedOnUsersRole = LoggedOnReadOnlyUser.GetRole(RoleService);

                // Get the global settings
                var settings = SettingsService.GetSettings();

                // Get allowed Groups
                var allowedGroups = _groupService.GetAllowedGroups(loggedOnUsersRole, LoggedOnReadOnlyUser?.Id);


                // Set the page index
                var pageIndex = p ?? 1;

                // Get all the topics based on the search value
                var posts = await _postService.SearchPosts(pageIndex,
                    ForumConfiguration.Instance.SearchListSize,
                    int.MaxValue,
                    siteSearch,
                    allowedGroups);

                // Get all the permissions for these topics
                var topicPermissions =
                    ViewModelMapping.GetPermissionsForTopics(posts.Select(x => x.Topic), RoleService,
                        loggedOnUsersRole);

                // Get the post Ids
                var postIds = posts.Select(x => x.Id).ToList();

                // Get all votes for these posts
                var votes = _voteService.GetVotesByPosts(postIds);

                // Get all favourites for these posts
                var favs = _favouriteService.GetAllPostFavourites(postIds);

                // Create the post view models
                var viewModels = ViewModelMapping.CreatePostViewModels(posts.ToList(), votes, topicPermissions,
                    LoggedOnReadOnlyUser, settings, favs);

                // create the view model
                var viewModel = new SearchViewModel
                {
                    Posts = viewModels,
                    PageIndex = pageIndex,
                    TotalCount = posts.TotalCount,
                    TotalPages = posts.TotalPages,
                    Term = siteSearch
                };

                return View(viewModel);
            }

            return RedirectToAction("Index", "Home");
        }


        [ChildActionOnly]
        public virtual PartialViewResult SideSearch()
        {
            return PartialView();
        }
    }
}