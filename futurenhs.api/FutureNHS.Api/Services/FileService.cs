using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class FileService : IFileService
    {
        private const string DefaultRole = "Standard Members";
        private const string AddDiscussionRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/add";

        private readonly ILogger<DiscussionService> _logger;
        private readonly IFileCommand _fileCommand;
        private readonly IGroupCommand _groupCommand;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;

        public FileService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, IFileCommand fileCommand, IGroupCommand groupCommand)
        {
            _systemClock = systemClock;
            _fileCommand = fileCommand;
            _groupCommand = groupCommand;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public async Task CreateFileAsync(Guid userId, string slug, Guid folderId, FutureNHS.Api.Models.File.File file, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;


            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateFileAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var fileStatus = await _fileCommand.GetFileStatus("Verified",cancellationToken);

            var fileDto = new FileDto()
            {
                Title = file.Title,
                Description = file.Description,
                FileName = file.FileName,
                FileSizeBytes = file.FileSizeInBytes,
                FileExtension  = file.FileExtension,
                BlobName = file.BlobName,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                FileStatus = fileStatus,
                ParentFolder = folderId,
                BlobHash  = new byte[100],
                IsDeleted = false
            };

            await _fileCommand.CreateFileAsync(fileDto, cancellationToken);
        }

    }
}
