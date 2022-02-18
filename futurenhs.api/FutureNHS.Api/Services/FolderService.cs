using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Folder;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class FolderService : IFolderService
    {
        private const string DefaultRole = "Standard Members";
        private const string AddDiscussionRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/add";

        private readonly ILogger<DiscussionService> _logger;
        private readonly IFolderCommand _folderCommand;
        private readonly IGroupCommand _groupCommand;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;

        public FolderService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, IFolderCommand folderCommand, IGroupCommand groupCommand)
        {
            _systemClock = systemClock;
            _folderCommand = folderCommand;
            _groupCommand = groupCommand;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public async Task CreateFolderAsync(Guid userId, string slug, Folder folder, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: CreateFolderAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupId.Value, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateFolderAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var folderDto = new FolderDto()
            {
                Title = folder.Title,
                Description = folder.Description,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                ParentFolder = null,
                GroupId = groupId.Value,
                IsDeleted = false
            };

            await _folderCommand.CreateFolderAsync(userId, groupId.Value, folderDto, cancellationToken);
        }

        public async Task CreateChildFolderAsync(Guid userId, string slug, Guid parentFolderId, Folder folder, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: CreateChildFolderAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupId.Value, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateChildFolderAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var folderDto = new FolderDto()
            {
                Title = folder.Title,
                Description = folder.Description,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                ParentFolder = parentFolderId,
                GroupId = groupId.Value,
                IsDeleted = false
            };

            await _folderCommand.CreateFolderAsync(userId, groupId.Value, folderDto, cancellationToken);
        }
    }
}
