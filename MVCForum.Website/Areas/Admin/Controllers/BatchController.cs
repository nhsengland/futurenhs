namespace MvcForum.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Web.ViewModels;
    using Web.ViewModels.Admin;

    [Authorize(Roles = Constants.AdminRoleName)]
    public class BatchController : BaseAdminController
    {
        private readonly IGroupService _GroupService;
        private readonly IPrivateMessageService _privateMessageService;
        private readonly ITopicService _topicService;

        public BatchController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, ISettingsService settingsService,
            IGroupService GroupService, ITopicService topicService, IPrivateMessageService privateMessageService,
            IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, settingsService, context)
        {
            _GroupService = GroupService;
            _topicService = topicService;
            _privateMessageService = privateMessageService;
        }

        #region Members

        public ActionResult BatchDeleteMembers()
        {
            return View(new BatchDeleteMembersViewModel { AmoutOfDaysSinceRegistered = 0, AmoutOfPosts = 0 });
        }

        /// <summary>
        /// Batch delete members
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchDeleteMembers(BatchDeleteMembersViewModel viewModel)
        {

            var membersToDelete = MembershipService.GetUsersByDaysPostsPoints(
                viewModel.AmoutOfDaysSinceRegistered,
                viewModel.AmoutOfPosts);

            var count = membersToDelete.Count;
            foreach (var membershipUser in membersToDelete)
            {
                var pipelineResult = await MembershipService.Delete(membershipUser);
                if (!pipelineResult.Successful)
                {
                    TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = pipelineResult.ProcessLog.FirstOrDefault(),
                        MessageType = GenericMessages.danger
                    };
                    return View();
                }
            }

            TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = $"{count} members deleted",
                MessageType = GenericMessages.success
            };

            return View();
        }

        #endregion

        #region Topics

        public ActionResult BatchMoveTopics()
        {
            var viewModel = new BatchMoveTopicsViewModel
            {
                Groups = _GroupService.GetAll()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchMoveTopics(BatchMoveTopicsViewModel viewModel)
        {
            try
            {
                var GroupFrom = _GroupService.Get((Guid)viewModel.FromGroup);
                var GroupTo = _GroupService.Get((Guid)viewModel.ToGroup);

                var topicsToMove = _topicService.GetRssTopicsByGroup(int.MaxValue, GroupFrom.Id);
                var count = topicsToMove.Count;

                foreach (var topic in topicsToMove)
                {
                    topic.Group = GroupTo;
                }

                Context.SaveChanges();

                GroupFrom.Topics.Clear();

                viewModel.Groups = _GroupService.GetAll();

                Context.SaveChanges();

                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = $"{count} topics moved",
                    MessageType = GenericMessages.success
                };
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = ex.Message,
                    MessageType = GenericMessages.danger
                };
            }


            return View(viewModel);
        }

        #endregion

        #region Private Messages

        public ActionResult BatchDeletePrivateMessages()
        {
            var viewModel = new BatchDeletePrivateMessagesViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchDeletePrivateMessages(BatchDeletePrivateMessagesViewModel viewModel)
        {
            try
            {
                var pms = _privateMessageService.GetPrivateMessagesOlderThan(viewModel.Days);
                var pmToDelete = new List<PrivateMessage>();
                pmToDelete.AddRange(pms);
                var count = pmToDelete.Count;
                foreach (var pm in pmToDelete)
                {
                    _privateMessageService.DeleteMessage(pm);
                }
                Context.SaveChanges();

                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = $"{count} Private Messages deleted",
                    MessageType = GenericMessages.success
                };
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = ex.Message,
                    MessageType = GenericMessages.danger
                };
            }


            return View(viewModel);
        }

        #endregion
    }
}