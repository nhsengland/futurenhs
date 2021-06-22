namespace MvcForum.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using ViewModels.Poll;

    [Authorize]
    public partial class PollController : BaseController
    {
        private readonly IPollService _pollService;

        public PollController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IPollService pollService, ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _pollService = pollService;
        }

        [HttpPost]
        public virtual PartialViewResult UpdatePoll(UpdatePollViewModel updatePollViewModel)
        {
            try
            {
                User.GetMembershipUser(MembershipService);

                // Fist need to check this user hasn't voted already and is trying to fudge the system
                if (LoggedOnReadOnlyUser != null && !_pollService.HasUserVotedAlready(updatePollViewModel.AnswerId, LoggedOnReadOnlyUser.Id))
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser?.Id);

                    // Get the answer
                    var pollAnswer = _pollService.GetPollAnswer(updatePollViewModel.AnswerId);

                    // create a new vote
                    var pollVote = new PollVote {PollAnswer = pollAnswer, User = loggedOnUser};

                    // Add it
                    _pollService.Add(pollVote);

                    // Update the context so the changes are reflected in the viewmodel below
                    Context.SaveChanges();
                }

                // Create the view model and get ready return the poll partial view
                var poll = _pollService.Get(updatePollViewModel.PollId);
                var votes = poll.PollAnswers.SelectMany(x => x.PollVotes).ToList();
                var alreadyVoted = votes.Count(x => x.User.Id == LoggedOnReadOnlyUser?.Id) > 0;
                var viewModel = new PollViewModel
                {
                    Poll = poll,
                    TotalVotesInPoll = votes.Count(),
                    UserHasAlreadyVoted = alreadyVoted
                };

                // Commit the transaction
                Context.SaveChanges();

                return PartialView("_Poll", viewModel);
            }
            catch (Exception ex)
            {
                Context.RollBack();
                LoggingService.Error(ex);
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }
    }
}