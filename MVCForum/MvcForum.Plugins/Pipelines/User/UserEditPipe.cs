namespace MvcForum.Plugins.Pipelines.User
{
    using System;
    using System.Data.Entity;
    using System.IO;
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

    public class UserEditPipe : IPipe<IPipelineProcess<MembershipUser>>
    {
        private readonly IMembershipService _membershipService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityService _activityService;
        private readonly ILoggingService _loggingService;

        public UserEditPipe(IMembershipService membershipService, ILocalizationService localizationService, IActivityService activityService, ILoggingService loggingService)
        {
            _membershipService = membershipService;
            _localizationService = localizationService;
            _activityService = activityService;
            _loggingService = loggingService;
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
                        var uploadResult = avatar.UploadFile(uploadFolderPath, _localizationService, true);

                        // throw error if unsuccessful
                        if (!uploadResult.UploadSuccessful)
                        {
                            input.AddError(uploadResult.ErrorMessage);
                            return input;
                        }

                        // Set the existing image for removing the file later, i.e. delete old image if there is one
                        existingImage = input.EntityToProcess.Avatar;

                        // Save avatar
                        input.EntityToProcess.Avatar = uploadResult.UploadedFileName;
                    }
                }

                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.ImageToRemove) && input.ExtendedData[Constants.ExtendedDataKeys.ImageToRemove] is string imageToRemove && imageToRemove == input.EntityToProcess.Avatar)
                {
                    existingImage = imageToRemove;
                    input.EntityToProcess.Avatar = null;
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
                        input.AddError(_localizationService.GetResourceString("Members.Errors.DuplicateUserName"));
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
                        input.AddError(_localizationService.GetResourceString("Members.Errors.DuplicateEmail"));
                        return input;
                    }
                }

                // Add an activity
                _activityService.ProfileUpdated(input.EntityToProcess);

                // Save the user
                var saved = await context.SaveChangesAsync();
                if (saved <= 0)
                {
                    input.AddError(_localizationService.GetResourceString("Errors.GenericMessage"));
                }

                // Delete existing image if requested or if new image uploaded
                if (!string.IsNullOrWhiteSpace(existingImage))
                {
                    var (Success, Message) = DeletePhysicalProfileImage(uploadFolderPath, existingImage);
                    if (!Success)
                    {
                        input.AddError(string.IsNullOrWhiteSpace(Message) ? "Unknown error removing existing profile image" : Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            return input;
        }

        private (bool Success, string Message) DeletePhysicalProfileImage(string filePath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            var fullFilePath = $"{filePath}\\{fileName}";
            try
            {
                if (!Directory.Exists(filePath))
                {
                    return (false, "Directory does not exist for user");
                }
                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                    return (true, null);
                }
                else
                {
                    return (false, "Image does not exist for user");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}