namespace MvcForum.Plugins.Pipelines.Group
{
    using System;
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

    public class GroupFilesPipe : IPipe<IPipelineProcess<Group>>
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        private readonly IImageService _imageService;
        private readonly IImageCommand _imageCommand;
        private readonly IImageRepository _imageRepository;

        public GroupFilesPipe(ILocalizationService localizationService, ILoggingService loggingService, IImageService imageService,
            IImageCommand imageCommand, IImageRepository imageRepository)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageCommand = imageCommand ?? throw new ArgumentNullException(nameof(imageCommand));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<Group>> Process(IPipelineProcess<Group> input,
            IMvcForumContext context)
        {
            _localizationService.RefreshContext(context);

            try
            {
                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.PostedFiles))
                {
                    // Sort image out first
                    if (input.ExtendedData[Constants.ExtendedDataKeys.PostedFiles] is HttpPostedFileBase[] files)
                    {
                        // Before we save anything, check the user already has an upload folder and if not create one
                        var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(ForumConfiguration.Instance.UploadFolderPath, input.EntityToProcess.Id));
                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath ?? throw new InvalidOperationException());
                        }

                        // Loop through each file and get the file info and save to the users folder and Db
                        var file = files[0];
                        if (file != null)
                        {
                            var uploadResult = file.UploadFile(uploadFolderPath, _localizationService, _imageCommand, _imageRepository, _imageService, false, input.EntityToProcess.Id);                            

                            // Save avatar to user
                            input.EntityToProcess.Image = uploadResult.UploadedFileName;

                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            return input;
        }
    }
}