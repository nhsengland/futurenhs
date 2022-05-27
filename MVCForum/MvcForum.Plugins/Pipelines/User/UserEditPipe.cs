namespace MvcForum.Plugins.Pipelines.User
{
    using System;
    using System.Data.Entity;
    using System.IO;
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
    using Core.Models.Entities;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;

    public class UserEditPipe : IPipe<IPipelineProcess<MembershipUser>>
    {
        private readonly IMembershipService _membershipService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityService _activityService;
        private readonly ILoggingService _loggingService;
        private readonly IImageService _imageService;
        private readonly IImageCommand _imageCommand;
        private readonly IImageRepository _imageRepository;

        public UserEditPipe(IMembershipService membershipService, ILocalizationService localizationService, 
            IActivityService activityService, ILoggingService loggingService, IImageService imageService,
            IImageCommand imageCommand, IImageRepository imageRepository)
        {
            _membershipService = membershipService;
            _localizationService = localizationService;
            _activityService = activityService;
            _loggingService = loggingService;
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageCommand = imageCommand ?? throw new ArgumentNullException(nameof(imageCommand));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<MembershipUser>> Process(IPipelineProcess<MembershipUser> input,
            IMvcForumContext context)
        {
            _membershipService.RefreshContext(context);
            _localizationService.RefreshContext(context);
            _activityService.RefreshContext(context);

            try
            {
                var existingImage = string.Empty;
                var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(ForumConfiguration.Instance.UploadFolderPath, input.EntityToProcess.Id));

                // Grab out the image if we have one
                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.PostedFiles))
                {
                    // Check we're good
                    if (input.ExtendedData[Constants.ExtendedDataKeys.PostedFiles] is HttpPostedFileBase avatar)
                    {
                        // Before we save anything, check the user already has an upload folder and if not create one
                        // If successful then upload the file                    
                        var uploadResult = avatar.UploadFile(uploadFolderPath, _localizationService, _imageCommand, _imageRepository, _imageService, true, input.EntityToProcess.Id);

                        // throw error if unsuccessful
                        if (!uploadResult.UploadSuccessful)
                        {
                            input.AddError(uploadResult.ErrorMessage);
                            return input;
                        }

                    }
                }

                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.ImageToRemove) && input.ExtendedData[Constants.ExtendedDataKeys.ImageToRemove] is string imageToRemove )
                {
                    existingImage = imageToRemove;
                }

                // Edit the user now - Get the original from the database
                var dbUser = await context.MembershipUser.FirstOrDefaultAsync(x => x.Id == input.EntityToProcess.Id);

                // User is trying to change username, need to check if a user already exists
                // with the username they are trying to change to
                var changedUsername = false;
                if (dbUser.UserName != input.EntityToProcess.UserName)
                {
                    if (_membershipService.GetUser(input.EntityToProcess.UserName) != null)
                    {
                        input.AddError("This email address is already registered.  Please provide another email address or login.");
                        return input;
                    }
                    changedUsername = true;
                }

                // Add username changed to extended data
                input.ExtendedData.Add(Constants.ExtendedDataKeys.UsernameChanged, changedUsername);

                // User is trying to update their email address, need to 
                // check the email is not already in use
                if (dbUser.Email != input.EntityToProcess.Email)
                {
                    // Add get by email address
                    if (_membershipService.GetUserByEmail(input.EntityToProcess.Email) != null)
                    {
                        input.AddError("This email address is already registered.  Please provide another email address or login.");
                        return input;
                    }
                }

                // Add an activity
                _activityService.ProfileUpdated(input.EntityToProcess);

                // Save the user
                var saved = await context.SaveChangesAsync();
                if (saved <= 0)
                {
                    input.AddError("Sorry an error occured");
                }
            }
            catch (System.Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            return input;
        }
    }
}