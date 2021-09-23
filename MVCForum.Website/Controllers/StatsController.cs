namespace MvcForum.Web.Controllers
{
    using System.Web.Mvc;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Enums;
    using ViewModels.Stats;

    [Authorize]
    public partial class StatsController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;

        public StatsController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            ITopicService topicService, IPostService postService, IGroupService GroupService,
            ICacheService cacheService,
            IMvcForumContext context) :
            base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicService = topicService;
            _postService = postService;
            _groupService = GroupService;
        }

        [ChildActionOnly]
        [OutputCache(Duration = (int) CacheTimes.OneHour)]
        public virtual PartialViewResult GetMainStats()
        {
            var allCats = _groupService.GetAll(LoggedOnReadOnlyUser?.Id);
            var viewModel = new MainStatsViewModel
            {
                LatestMembers = MembershipService.GetLatestUsers(10),
                MemberCount = MembershipService.MemberCount(),
                TopicCount = _topicService.TopicCount(allCats),
                PostCount = _postService.PostCount(allCats)
            };
            return PartialView(viewModel);
        }
    }
}