namespace MvcForum.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Enums;
    using ViewModels.Tag;

    public partial class TagController : BaseController
    {
        private readonly IGroupService _GroupService;
        private readonly ITopicTagService _topicTagService;

        public TagController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            ITopicTagService topicTagService, IGroupService GroupService, ICacheService cacheService,
            IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicTagService = topicTagService;
            _GroupService = GroupService;
        }

        [ChildActionOnly]
        public virtual PartialViewResult PopularTags(int amountToTake)
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            var cacheKey = string.Concat("PopularTags", amountToTake, loggedOnUsersRole.Id);
            var viewModel = CacheService.Get<PopularTagViewModel>(cacheKey);
            if (viewModel == null)
            {
                var allowedGroups = _GroupService.GetAllowedGroups(loggedOnUsersRole);
                var popularTags = _topicTagService.GetPopularTags(amountToTake, allowedGroups);
                viewModel = new PopularTagViewModel {PopularTags = popularTags};
                CacheService.Set(cacheKey, viewModel, CacheTimes.SixHours);
            }
            return PartialView(viewModel);
        }

        [HttpGet]
        public virtual JsonResult AutoCompleteTags(string term)
        {
            var returnList = new List<string>();
            var tags = _topicTagService.GetContains(term);

            if (!tags.Any())
            {
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }

            foreach (var topicTag in tags)
            {
                returnList.Add(topicTag.Tag);
            }

            return Json(returnList, JsonRequestBehavior.AllowGet);
        }
    }
}