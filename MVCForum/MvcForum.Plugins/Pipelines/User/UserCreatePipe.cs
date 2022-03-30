namespace MvcForum.Plugins.Pipelines.User
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using Core;
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Pipeline;
    using Core.Interfaces.Services;
    using Core.Models;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;

    public class UserCreatePipe : IPipe<IPipelineProcess<MembershipUser>>
    {
        private readonly IActivityService _activityService;
        private readonly IEmailService _emailService;
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        private readonly IMembershipService _membershipService;
        private readonly ISettingsService _settingsService;
        private readonly IGroupInviteService _groupInviteService;
        private readonly IGroupService _groupService;
        private readonly IImageService _imageService;
        private readonly IImageCommand _imageCommand;
        private readonly IImageRepository _imageRepository;

        public UserCreatePipe(IMembershipService membershipService, ILoggingService loggingService,
            ISettingsService settingsService, ILocalizationService localizationService, IEmailService emailService,
            IActivityService activityService, IGroupInviteService groupInviteService, IGroupService groupService,
            IImageService imageService, IImageCommand imageCommand, IImageRepository imageRepository)
        {
            _membershipService = membershipService;
            _loggingService = loggingService;
            _settingsService = settingsService;
            _localizationService = localizationService;
            _emailService = emailService;
            _activityService = activityService;
            _groupInviteService = groupInviteService;
            _groupService = groupService;
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageCommand = imageCommand ?? throw new ArgumentNullException(nameof(imageCommand));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<MembershipUser>> Process(IPipelineProcess<MembershipUser> input,
            IMvcForumContext context)
        {
            _membershipService.RefreshContext(context);
            _settingsService.RefreshContext(context);
            _localizationService.RefreshContext(context);
            _emailService.RefreshContext(context);
            _activityService.RefreshContext(context);

            try
            {
                if (string.IsNullOrWhiteSpace(input.EntityToProcess.UserName))
                {
                    input.ProcessLog.Clear();
                    input.ProcessLog.Add(_membershipService.ErrorCodeToString(MembershipCreateStatus.InvalidUserName));
                    input.Successful = false;
                    return input;
                }

                // get by username
                if (_membershipService.GetUser(input.EntityToProcess.UserName, true) != null)
                {
                    input.ProcessLog.Clear();
                    input.ProcessLog.Add(_membershipService.ErrorCodeToString(MembershipCreateStatus.DuplicateUserName));
                    input.Successful = false;
                    return input;
                }

                // Add get by email address
                if (_membershipService.GetUserByEmail(input.EntityToProcess.Email, true) != null)
                {
                    input.ProcessLog.Clear();
                    input.ProcessLog.Add(_membershipService.ErrorCodeToString(MembershipCreateStatus.DuplicateEmail));
                    input.Successful = false;
                    return input;
                }

                if (string.IsNullOrWhiteSpace(input.EntityToProcess.Password))
                {
                    input.ProcessLog.Clear();
                    input.ProcessLog.Add(_membershipService.ErrorCodeToString(MembershipCreateStatus.InvalidPassword));
                    input.Successful = false;
                    return input;
                }

                // Get the settings
                var settings = _settingsService.GetSettings(false);
                var manuallyAuthoriseMembers = settings.ManuallyAuthoriseNewMembers;
                var memberEmailAuthorisationNeeded = settings.NewMemberEmailConfirmation == true;

                // If this is a social login, and memberEmailAuthorisationNeeded is true then we need to ignore it
                // and set memberEmailAuthorisationNeeded to false because the email addresses are validated by the social media providers
     
                if (manuallyAuthoriseMembers || memberEmailAuthorisationNeeded)
                {
                    input.EntityToProcess.IsApproved = false;
                }
                else
                {
                    input.EntityToProcess.IsApproved = true;
                }
                

                input.EntityToProcess = context.MembershipUser.Add(input.EntityToProcess);
                var saved = await context.SaveChangesAsync();
                if (saved <= 0)
                {
                    input.ProcessLog.Add("Unable to save changes to the database");
                    input.Successful = false;
                    return input;
                }

                var inviteMailAddress = new MailAddress(input.EntityToProcess.Email);

                foreach (var invite in await _groupInviteService.GetInvitesForGroupAsync(inviteMailAddress, CancellationToken.None))
                {
                    if (invite.GroupId.HasValue)
                    {
                        if (await _groupService.JoinGroupApproveAsync(invite.GroupId.Value, input.EntityToProcess.Id,
                                CancellationToken.None))
                        {
                            await _groupInviteService.DeleteInviteAsync(invite.Id, CancellationToken.None);
                        }
                    }
                }

                // Now add a memberjoined activity
                _activityService.MemberJoined(input.EntityToProcess);

                // Set manuallyAuthoriseMembers, memberEmailAuthorisationNeeded in extendeddata
                input.ExtendedData.Add(Constants.ExtendedDataKeys.ManuallyAuthoriseMembers, manuallyAuthoriseMembers);
                input.ExtendedData.Add(Constants.ExtendedDataKeys.MemberEmailAuthorisationNeeded, memberEmailAuthorisationNeeded);
            }
            catch (Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            if (input.Successful)
            {
                input.ProcessLog.Add("CreateNewUserPipe Successful");
            }

            return input;
        }
    }
}